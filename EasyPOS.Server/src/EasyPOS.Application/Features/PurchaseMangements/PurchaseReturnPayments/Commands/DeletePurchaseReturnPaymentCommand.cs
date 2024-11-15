using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;

namespace EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Commands;

public record DeletePurchaseReturnPaymentCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchaseReturnPayment;
}

internal sealed class DeletePurchaseReturnPaymentCommandHandler(
    IApplicationDbContext dbContext,
    IPurchaseReturnService purchaseService)
    : ICommandHandler<DeletePurchaseReturnPaymentCommand>

{
    public async Task<Result> Handle(DeletePurchaseReturnPaymentCommand request, CancellationToken cancellationToken)
    {
        var purchaseReturnPayment = await dbContext.PurchaseReturnPayments
            .FindAsync([request.Id], cancellationToken);

        if (purchaseReturnPayment is null) return Result.Failure(Error.NotFound(nameof(purchaseReturnPayment), ErrorMessages.EntityNotFound));

        var purchaseReturn = await dbContext.PurchaseReturns
            .FirstOrDefaultAsync(x => x.Id == purchaseReturnPayment.PurchaseReturnId, cancellationToken: cancellationToken);

        if (purchaseReturn is null) return Result.Failure(Error.NotFound(nameof(purchaseReturn), "PurchaseReturn Not Found."));

        dbContext.PurchaseReturnPayments.Remove(purchaseReturnPayment);

        purchaseReturn.PaidAmount -= purchaseReturnPayment.PayingAmount;
        purchaseReturn.DueAmount = purchaseReturn.GrandTotal - purchaseReturn.PaidAmount;

        await purchaseService.AdjustPurchaseReturnAsync(
            purchaseReturn, 
            purchaseReturnPayment.PayingAmount, 
            PurchaseReturnTransactionType.ReturnPaymentDelete, 
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    
}
