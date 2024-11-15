using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;

namespace EasyPOS.Application.Features.Purchases.PurchasePayments.Commands;

public record DeletePurchasePaymentCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchasePayment;
}

internal sealed class DeletePurchasePaymentCommandHandler(
    IApplicationDbContext dbContext,
    ISupplierService supplierService,
    IPurchaseService purchaseService)
    : ICommandHandler<DeletePurchasePaymentCommand>

{
    public async Task<Result> Handle(DeletePurchasePaymentCommand request, CancellationToken cancellationToken)
    {
        var purchasePayment = await dbContext.PurchasePayments
            .FindAsync([request.Id], cancellationToken);

        if (purchasePayment is null) return Result.Failure(Error.NotFound(nameof(purchasePayment), ErrorMessages.EntityNotFound));

        var purchase = await dbContext.Purchases
            .FirstOrDefaultAsync(x => x.Id == purchasePayment.PurchaseId, cancellationToken: cancellationToken);

        if (purchase is null) return Result.Failure(Error.NotFound(nameof(purchase), "PurchaseCreate Not Found."));

        dbContext.PurchasePayments.Remove(purchasePayment);

        purchase.PaidAmount -= purchasePayment.PayingAmount;
        purchase.DueAmount = purchase.GrandTotal - purchase.PaidAmount;

        await purchaseService.AdjustPurchaseAsync(
            purchase, 
            purchasePayment.PayingAmount, 
            PurchaseTransactionType.PaymentDelete, 
            cancellationToken);

        // Adjust supplier balance for payment deletion
        await supplierService.AdjustSupplierBalance(
            purchase.SupplierId, 
            purchasePayment.PayingAmount, 
            PurchaseTransactionType.PaymentDelete,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
