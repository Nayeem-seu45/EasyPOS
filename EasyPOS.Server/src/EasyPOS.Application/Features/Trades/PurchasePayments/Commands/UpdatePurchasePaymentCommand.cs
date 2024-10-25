using EasyPOS.Application.Features.Trades.Purchases.Shared;

namespace EasyPOS.Application.Features.Trades.PurchasePayments.Commands;

public record UpdatePurchasePaymentCommand(
    Guid Id,
    Guid PurchaseId, 
    decimal ReceivedAmount, 
    decimal PayingAmount, 
    decimal ChangeAmount, 
    Guid PaymentType, 
    string? Note
    ): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchasePayment;
}

internal sealed class UpdatePurchasePaymentCommandHandler(
    IApplicationDbContext dbContext,
    ICommonQueryService commonQueryService) 
    : ICommandHandler<UpdatePurchasePaymentCommand>
{
    public async Task<Result> Handle(UpdatePurchasePaymentCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.PurchasePayments.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        var purchase = await dbContext.Purchases
            .FirstOrDefaultAsync(x => x.Id == entity.PurchaseId, cancellationToken: cancellationToken);

        if(purchase is null) return Result.Failure(Error.NotFound(nameof(purchase), "Purchase Not Found."));

        var previousPaymentAmount = entity.PayingAmount;

        request.Adapt(entity);

        purchase.PaidAmount += (entity.PayingAmount - previousPaymentAmount);
        purchase.DueAmount = purchase.GrandTotal - purchase.PaidAmount;
        purchase.PaymentStatusId = await PurchaseSharedService.GetPurchasePaymentId(commonQueryService, purchase);


        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
