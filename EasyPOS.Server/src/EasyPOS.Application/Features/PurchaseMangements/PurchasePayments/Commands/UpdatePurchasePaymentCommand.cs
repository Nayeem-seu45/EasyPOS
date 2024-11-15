using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;

namespace EasyPOS.Application.Features.Purchases.PurchasePayments.Commands;

public record UpdatePurchasePaymentCommand(
    Guid Id,
    Guid PurchaseId,
    decimal ReceivedAmount,
    decimal PayingAmount,
    decimal ChangeAmount,
    Guid PaymentType,
    string? Note
    ) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchasePayment;
}

internal sealed class UpdatePurchasePaymentCommandHandler(
    IApplicationDbContext dbContext,
    ISupplierService supplierService,
    IPurchaseService purchaseService)
    : ICommandHandler<UpdatePurchasePaymentCommand>
{
    public async Task<Result> Handle(UpdatePurchasePaymentCommand request, CancellationToken cancellationToken)
    {
        var purchasePayment = await dbContext.PurchasePayments.FindAsync([request.Id], cancellationToken);

        if (purchasePayment is null) return Result.Failure(Error.NotFound(nameof(purchasePayment), ErrorMessages.EntityNotFound));

        var purchase = await dbContext.Purchases
            .FirstOrDefaultAsync(x => x.Id == purchasePayment.PurchaseId, cancellationToken: cancellationToken);

        if (purchase is null) return Result.Failure(Error.NotFound(nameof(purchase), "PurchaseCreate Not Found."));

        var previousPaymentAmount = purchasePayment.PayingAmount;

        request.Adapt(purchasePayment);

        var paymentDifference = purchasePayment.PayingAmount - previousPaymentAmount;

        //purchase.PaidAmount += paymentDifference;
        //purchase.DueAmount = purchase.GrandTotal - purchase.PaidAmount;
        //purchase.PaymentStatusId = await PurchaseSharedService.GetPurchasePaymentId(commonQueryService, purchase);

        // Update purchase payment fields
        await purchaseService.AdjustPurchaseAsync(
            purchase,
            paymentDifference,
            PurchaseTransactionType.PaymentUpdate,
            cancellationToken);


        // Adjust supplier financials based on the payment difference
        await supplierService.AdjustSupplierBalance(
            purchase.SupplierId, 
            paymentDifference, 
            PurchaseTransactionType.PaymentUpdate, 
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
