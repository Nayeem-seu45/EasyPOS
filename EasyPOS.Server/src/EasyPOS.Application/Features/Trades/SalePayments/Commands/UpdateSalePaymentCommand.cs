namespace EasyPOS.Application.Features.Trades.SalePayments.Commands;

public record UpdateSalePaymentCommand(
    Guid Id,
    Guid SaleId, 
    decimal ReceivedAmount, 
    decimal PayingAmount, 
    decimal ChangeAmount, 
    Guid PaymentType, 
    string? Note
    ): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SalePayment;
}

internal sealed class UpdateSalePaymentCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateSalePaymentCommand>
{
    public async Task<Result> Handle(UpdateSalePaymentCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.SalePayments.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        var sale = await dbContext.Sales
            .FirstOrDefaultAsync(x => x.Id == entity.SaleId, cancellationToken: cancellationToken);

        if(sale is null) return Result.Failure(Error.NotFound(nameof(sale), "Sale Not Found."));

        var previousPaymentAmount = entity.PayingAmount;

        request.Adapt(entity);

        sale.PaidAmount += entity.PayingAmount - previousPaymentAmount;
        sale.DueAmount = sale.GrandTotal - sale.PaidAmount;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
