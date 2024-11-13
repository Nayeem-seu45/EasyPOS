namespace EasyPOS.Application.Features.Sales.SalePayments.Commands;

public record DeleteSalePaymentCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SalePayment;
}

internal sealed class DeleteSalePaymentCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<DeleteSalePaymentCommand>
{
    public async Task<Result> Handle(DeleteSalePaymentCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the existing sale payment
        var entity = await dbContext.SalePayments
            .FindAsync([request.Id], cancellationToken);
        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        // Retrieve the associated sale
        var sale = await dbContext.Sales
            .FirstOrDefaultAsync(x => x.Id == entity.SaleId, cancellationToken);
        if (sale is null) return Result.Failure(Error.NotFound(nameof(sale), "Sale Not Found."));

        dbContext.SalePayments.Remove(entity);

        // Update sale amounts after removing the payment
        sale.PaidAmount -= entity.PayingAmount;
        sale.DueAmount = sale.GrandTotal - sale.PaidAmount;

        // Update the customer’s financial records
        var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == sale.CustomerId, cancellationToken);
        if (customer is null) return Result.Failure(Error.Failure(nameof(customer), "Customer not found."));

        // Adjust the customer’s total amounts
        customer.TotalDueAmount += entity.PayingAmount;
        customer.TotalPaidAmount -= entity.PayingAmount;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

