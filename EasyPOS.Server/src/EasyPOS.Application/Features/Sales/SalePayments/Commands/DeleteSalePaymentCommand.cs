namespace EasyPOS.Application.Features.Sales.SalePayments.Commands;

public record DeleteSalePaymentCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SalePayment;
}

internal sealed class DeleteSalePaymentCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<DeleteSalePaymentCommand>

{
    public async Task<Result> Handle(DeleteSalePaymentCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.SalePayments
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        var sale = await dbContext.Sales
            .FirstOrDefaultAsync(x => x.Id == entity.SaleId, cancellationToken: cancellationToken);

        if (sale is null) return Result.Failure(Error.NotFound(nameof(sale), "Sale Not Found."));

        dbContext.SalePayments.Remove(entity);

        sale.PaidAmount -= entity.PayingAmount;
        sale.DueAmount = sale.GrandTotal - sale.PaidAmount;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

}
