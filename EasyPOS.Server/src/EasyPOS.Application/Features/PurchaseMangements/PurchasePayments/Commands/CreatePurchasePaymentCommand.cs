using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.Purchases.PurchasePayments.Commands;

public record CreatePurchasePaymentCommand(
    Guid PurchaseId,
    decimal ReceivedAmount,
    decimal PayingAmount,
    decimal ChangeAmount,
    Guid PaymentType,
    string? Note
    ) : ICacheInvalidatorCommand<Guid>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Purchase}";
}

internal sealed class CreatePurchasePaymentCommandHandler(
    IApplicationDbContext dbContext,
    ISupplierService supplierService,
    IPurchaseService purchaseService) : ICommandHandler<CreatePurchasePaymentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePurchasePaymentCommand request, CancellationToken cancellationToken)
    {
        var purchasePayment = request.Adapt<PurchasePayment>();
        purchasePayment.PaymentDate = DateTime.Now;

        dbContext.PurchasePayments.Add(purchasePayment);

        var purchase = await dbContext.Purchases
            .FirstOrDefaultAsync(x => x.Id == purchasePayment.PurchaseId, cancellationToken: cancellationToken);

        if (purchase is null)
        {
            return Result.Failure<Guid>(Error.Failure(nameof(purchase), "Purchase Entity not found"));
        }

        // Update purchase payment fields
        await purchaseService.AdjustPurchaseAsync(
            purchase,
            purchasePayment.PayingAmount,
            PurchaseTransactionType.PaymentCreate,
            cancellationToken);

        // Adjust supplier financials for the new payment
        await supplierService.AdjustSupplierBalance(
            purchase.SupplierId, 
            purchasePayment.PayingAmount, 
            PurchaseTransactionType.PaymentCreate,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return purchasePayment.Id;
    }

}

