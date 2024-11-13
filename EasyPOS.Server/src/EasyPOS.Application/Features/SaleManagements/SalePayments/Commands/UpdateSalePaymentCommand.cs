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
    IApplicationDbContext dbContext) : ICommandHandler<UpdateSalePaymentCommand>
{
    public async Task<Result> Handle(UpdateSalePaymentCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the existing sale payment
        var entity = await dbContext.SalePayments.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        // Retrieve the associated sale
        var sale = await dbContext.Sales
            .FirstOrDefaultAsync(x => x.Id == entity.SaleId, cancellationToken);
        if (sale is null) return Result.Failure(Error.NotFound(nameof(sale), "Sale Not Found."));

        var previousPaymentAmount = entity.PayingAmount;

        // Adapt the updated values to the entity
        request.Adapt(entity);

        // Update sale amounts based on the new payment amount
        sale.PaidAmount += entity.PayingAmount - previousPaymentAmount;
        sale.DueAmount = sale.GrandTotal - sale.PaidAmount;

        // Update the customer’s financial records
        var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == sale.CustomerId, cancellationToken);
        if (customer is null) return Result.Failure(Error.Failure(nameof(customer), "Customer not found."));

        // Adjust the customer’s total amounts
        customer.TotalDueAmount += previousPaymentAmount - entity.PayingAmount;
        customer.TotalPaidAmount += entity.PayingAmount - previousPaymentAmount;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
