using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Application.Features.Stakeholders.Customers.Services;

namespace EasyPOS.Application.Features.Sales.SalePayments.Commands;

public record UpdateSalePaymentCommand(
    Guid Id,
    Guid SaleId,
    decimal ReceivedAmount,
    decimal PayingAmount,
    decimal ChangeAmount,
    Guid PaymentType,
    string? Note
    ) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SalePayment;
}

internal sealed class UpdateSalePaymentCommandHandler(
    IApplicationDbContext dbContext,
    ISaleService saleService,
    ICustomerService customerService)
    : ICommandHandler<UpdateSalePaymentCommand>
{
    public async Task<Result> Handle(UpdateSalePaymentCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the existing sale payment
        var salePayment = await dbContext.SalePayments
            .FindAsync([request.Id], cancellationToken);

        if (salePayment is null)
            return Result.Failure(Error.NotFound(nameof(salePayment), ErrorMessages.EntityNotFound));

        // Retrieve the associated sale
        var sale = await dbContext.Sales
            .FirstOrDefaultAsync(x => x.Id == salePayment.SaleId, cancellationToken);

        if (sale is null)
            return Result.Failure(Error.NotFound(nameof(sale), "Sale not found."));

        // Calculate the previous payment amount
        var previousPaymentAmount = salePayment.PayingAmount;

        // Update payment entity with new values
        request.Adapt(salePayment);

        // Adjust sale based on payment update
        var paymentDifference = salePayment.PayingAmount - previousPaymentAmount;

        await saleService.AdjustSaleAsync(
            SaleTransactionType.PaymentUpdate,
            sale,
            paymentDifference,
            cancellationToken);

        // Retrieve the associated customer
        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == sale.CustomerId, cancellationToken);

        if (customer is null)
            return Result.Failure(Error.Failure(nameof(customer), "Customer not found."));

        // Adjust customer's financial records based on the payment update
        customerService.AdjustCustomerOnPayment(
            SaleTransactionType.PaymentUpdate,
            customer,
            paymentDifference);

        // Save changes to the database
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
