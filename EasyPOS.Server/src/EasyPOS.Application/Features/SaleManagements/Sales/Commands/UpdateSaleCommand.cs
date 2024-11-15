using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Application.Features.Sales.Models;
using EasyPOS.Application.Features.Stakeholders.Customers.Services;

namespace EasyPOS.Application.Features.Sales.Commands;

public record UpdateSaleCommand : UpsertSaleModel, ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Sale;
}

internal sealed class UpdateSaleCommandHandler(
    IApplicationDbContext dbContext,
    ISaleService saleService,
    ICustomerService customerService,
    ICommonQueryService commonQueryService)
    : ICommandHandler<UpdateSaleCommand>
{
    public async Task<Result> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the existing sale entity
        var entity = await dbContext.Sales
            .Include(s => s.SalePayments) // Load SalePayments to verify payments cannot be modified
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));
        }

        // Calculate the original due and paid amounts for adjusting the customer's record
        var previousDueAmount = entity.DueAmount;
        var previousPaidAmount = entity.PaidAmount;

        // Adapt the non-payment properties from the request
        request.Adapt(entity);

        // Use SaleService to adjust sale due and payment status
        await saleService.AdjustSaleAsync(SaleTransactionType.SaleUpdate, entity, 0, cancellationToken);

        // Update the customer's due and paid amounts via CustomerService
        var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == entity.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result.Failure(Error.Failure(nameof(customer), "Customer not found."));
        }

        // Adjust customer totals by removing the old amounts and adding the new amounts
        customerService.AdjustCustomerOnSale(
            SaleTransactionType.SaleUpdate,
            customer,
            entity.DueAmount - previousDueAmount
        );

        // Save all changes
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
