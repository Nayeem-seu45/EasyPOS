using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;

namespace EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Commands;

public record UpdatePurchaseReturnPaymentCommand(
    Guid Id,
    Guid PurchaseReturnId,
    decimal ReceivedAmount,
    decimal PayingAmount,
    decimal ChangeAmount,
    Guid PaymentType,
    string? Note
    ) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchaseReturnPayment;
}

internal sealed class UpdatePurchaseReturnPaymentCommandHandler(
    IApplicationDbContext dbContext,
    IPurchaseReturnService purchaseService)
    : ICommandHandler<UpdatePurchaseReturnPaymentCommand>
{
    public async Task<Result> Handle(UpdatePurchaseReturnPaymentCommand request, CancellationToken cancellationToken)
    {
        var purchaseReturnPayment = await dbContext.PurchaseReturnPayments.FindAsync([request.Id], cancellationToken);

        if (purchaseReturnPayment is null) return Result.Failure(Error.NotFound(nameof(purchaseReturnPayment), ErrorMessages.EntityNotFound));

        var purchaseReturn = await dbContext.PurchaseReturns
            .FirstOrDefaultAsync(x => x.Id == purchaseReturnPayment.PurchaseReturnId, cancellationToken: cancellationToken);

        if (purchaseReturn is null) return Result.Failure(Error.NotFound(nameof(purchaseReturn), "PurchaseReturn Not Found."));

        var previousPaymentAmount = purchaseReturnPayment.PayingAmount;

        request.Adapt(purchaseReturnPayment);

        var paymentDifference = purchaseReturnPayment.PayingAmount - previousPaymentAmount;

        // Update purchase payment fields
        await purchaseService.AdjustPurchaseReturnAsync(
            purchaseReturn,
            paymentDifference,
            PurchaseReturnTransactionType.ReturnPaymentUpdate,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
