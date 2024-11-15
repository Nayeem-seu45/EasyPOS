using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Application.Features.Purchases.Models;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;
using EasyPOS.Domain.Common.Enums;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.Purchases.Commands;

public record CreatePurchaseCommand(
    DateOnly PurchaseDate,
    Guid WarehouseId,
    Guid SupplierId,
    Guid PurchaseStatusId,
    string? AttachmentUrl,
    decimal SubTotal,
    decimal? TaxRate,
    decimal? TaxAmount,
    DiscountType DiscountType,
    decimal? DiscountRate,
    decimal? DiscountAmount,
    decimal? ShippingCost,
    decimal GrandTotal,
    string? Note,
    List<PurchaseDetailModel> PurchaseDetails
    ) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Purchase;
}

internal sealed class CreatePurchaseCommandHandler(
    IApplicationDbContext dbContext,
    ISupplierService supplierFinancialService,
    IPurchaseService purchaseService)
    : ICommandHandler<CreatePurchaseCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = request.Adapt<Purchase>();
        //entity.DueAmount = entity.GrandTotal;
        //entity.PurchaseStatusId = await purchaseService.GetPurchasePaymentId(entity) ?? Guid.Empty;
        dbContext.Purchases.Add(purchase);
        purchase.ReferenceNo = "PUR-" + DateTime.Now.ToString("yyyyMMddhhmmffff");

        await purchaseService.AdjustPurchaseAsync(purchase, 0, PurchaseTransactionType.PurchaseCreate, cancellationToken);

        // Adjust supplier financials
        await supplierFinancialService.AdjustSupplierBalance(
            purchase.SupplierId, 
            purchase.DueAmount, 
            PurchaseTransactionType.PurchaseCreate, 
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(purchase.Id);
    }
}
