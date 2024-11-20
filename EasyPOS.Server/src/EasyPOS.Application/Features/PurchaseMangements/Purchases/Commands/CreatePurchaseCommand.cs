using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Application.Features.Purchases.Models;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;
using EasyPOS.Application.Features.StockManagement.Services;
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
    ISupplierService supplierService,
    IPurchaseService purchaseService,
    IStockService stockService)
    : ICommandHandler<CreatePurchaseCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = request.Adapt<Purchase>();
        dbContext.Purchases.Add(purchase);
        purchase.ReferenceNo = "PUR-" + DateTime.Now.ToString("yyyyMMddhhmmffff");

        await purchaseService.AdjustPurchaseAsync(purchase, 0, PurchaseTransactionType.PurchaseCreate, cancellationToken);

        // Adjust stock for each purchased item
        foreach (var item in purchase.PurchaseDetails)
        {
            await stockService.AdjustStockAsync(
                productId: item.ProductId,
                warehouseId: purchase.WarehouseId,
                quantity: item.Quantity,
                unitCost: item.NetUnitCost, // Use per-unit cost for accuracy
                isAddition: true // Adding stock
            );
        }


        // Adjust supplier financials
        await supplierService.AdjustSupplierBalance(
            purchase.SupplierId,
            purchase.DueAmount,
            PurchaseTransactionType.PurchaseCreate,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(purchase.Id);
    }
}
