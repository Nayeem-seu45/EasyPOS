using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Commands;

public record CreatePurchaseReturnPaymentCommand(
    Guid PurchaseReturnId,
    decimal ReceivedAmount,
    decimal PayingAmount,
    decimal ChangeAmount,
    Guid PaymentType,
    string? Note
    ) : ICacheInvalidatorCommand<Guid>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.PurchaseReturn}";
}

internal sealed class CreatePurchaseReturnPaymentCommandHandler(
    IApplicationDbContext dbContext,
    IPurchaseReturnService purchaseReturnService) : ICommandHandler<CreatePurchaseReturnPaymentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePurchaseReturnPaymentCommand request, CancellationToken cancellationToken)
    {
        var purchaseReturnPayment = request.Adapt<PurchaseReturnPayment>();
        purchaseReturnPayment.PaymentDate = DateTime.Now;

        dbContext.PurchaseReturnPayments.Add(purchaseReturnPayment);

        var purchaseReturn = await dbContext.PurchaseReturns
            .FirstOrDefaultAsync(x => x.Id == purchaseReturnPayment.PurchaseReturnId, cancellationToken: cancellationToken);

        if (purchaseReturn is null)
        {
            return Result.Failure<Guid>(Error.Failure(nameof(purchaseReturn), "PurchaseReturn Entity not found"));
        }

        // Update purchase return  payment fields
        await purchaseReturnService.AdjustPurchaseReturnAsync(
            purchaseReturn,
            purchaseReturnPayment.PayingAmount,
            PurchaseReturnTransactionType.ReturnPaymentCreate,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return purchaseReturnPayment.Id;
    }

}

