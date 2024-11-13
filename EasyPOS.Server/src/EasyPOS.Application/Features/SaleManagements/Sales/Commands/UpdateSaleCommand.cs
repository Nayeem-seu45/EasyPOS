using EasyPOS.Application.Common.Enums;
using EasyPOS.Application.Features.Sales.Models;
using EasyPOS.Domain.Common;

namespace EasyPOS.Application.Features.Sales.Commands;

public record UpdateSaleCommand : UpsertSaleModel, ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Sale;
}

internal sealed class UpdateSaleCommandHandler(
    IApplicationDbContext dbContext,
    ICommonQueryService commonQueryService)
    : ICommandHandler<UpdateSaleCommand>
{
    public async Task<Result> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the existing sale entity
        var entity = await dbContext.Sales
            .Include(s => s.SalePayments) // Load SalePayments but do not modify them
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

        // Update DueAmount and adjust based on the current GrandTotal
        entity.DueAmount = entity.GrandTotal - entity.PaidAmount;

        // Fetch SalePaymentStatus lookup list
        var salePaymentStatusList = await commonQueryService
            .GetLookupDetailsAsync((int)LookupDevCode.SalePaymentStatus, cancellationToken);

        // Update the PaymentStatusId based on current DueAmount and PaidAmount
        if (entity.DueAmount == 0)
        {
            entity.PaymentStatusId = GetSalePaymentStatusId(salePaymentStatusList, SalePaymentStatus.Paid);
        }
        else if (entity.PaidAmount > 0)
        {
            entity.PaymentStatusId = GetSalePaymentStatusId(salePaymentStatusList, SalePaymentStatus.Partial);
        }
        else
        {
            entity.PaymentStatusId = GetSalePaymentStatusId(salePaymentStatusList, SalePaymentStatus.Pending);
        }

        // Update the customer's due and paid amounts
        var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == entity.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result.Failure(Error.Failure(nameof(customer), "Customer not found."));
        }

        // Adjust customer totals by removing the old amounts and adding the new amounts
        customer.TotalDueAmount = customer.TotalDueAmount - previousDueAmount + entity.DueAmount;
        customer.TotalPaidAmount = customer.TotalPaidAmount - previousPaidAmount + entity.PaidAmount;

        // Save all changes
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private Guid GetSalePaymentStatusId(List<LookupDetail> lookupDetails, SalePaymentStatus paymentStatus)
    {
        return lookupDetails.FirstOrDefault(x => x.DevCode == (int)paymentStatus)?.Id ?? Guid.Empty;
    }
}
