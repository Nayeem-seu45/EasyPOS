using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Application.Features.PurchaseReturns.Models;
using EasyPOS.Application.Features.StockManagement.Services;
using EasyPOS.Domain.Common.Enums;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.PurchaseReturns.Commands;

public record CreatePurchaseReturnCommand(
    DateOnly ReturnDate,
    Guid PurchaseId,
    Guid ReturnStatusId,
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
    List<PurchaseReturnDetailModel> PurchaseReturnDetails
    ) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.PurchaseReturn;
}

internal sealed class CreatePurchaseReturnCommandHandler(
    IApplicationDbContext dbContext,
    IPurchaseReturnService purchaseReturnService,
    IStockService stockService)
    : ICommandHandler<CreatePurchaseReturnCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePurchaseReturnCommand request, CancellationToken cancellationToken)
    {
        var purchaseReturn = request.Adapt<PurchaseReturn>();
        dbContext.PurchaseReturns.Add(purchaseReturn);

        purchaseReturn.ReferenceNo = "PR-" + DateTime.Now.ToString("yyyyMMddhhmmffff");

        var mapResult = await MapPurchaseToPurchaseReturn(dbContext, purchaseReturn);
        if (!mapResult.IsSuccess)
        {
            return Result.Failure<Guid>(mapResult.Error);
        }

        // Adjust stock for each item in the purchase return
        foreach (var item in purchaseReturn.PurchaseReturnDetails)
        {
            if (item.ReturnedQuantity <= 0)
                return Result.Failure<Guid>(Error.Failure("Invalid Operation Exception", $"Invalid returned quantity for product {item.ProductName}."));

            await stockService.AdjustStockOnPurchaseAsync(
                productId: item.ProductId,
                warehouseId: purchaseReturn.WarehouseId,
                quantity: item.ReturnedQuantity,
                unitCost: item.NetUnitCost,
                isAddition: false // Reduce stock due to purchase return
            );
        }

        await purchaseReturnService.AdjustPurchaseReturnAsync(
            purchaseReturn,
            0,
            PurchaseReturnTransactionType.PurchaseReturnCreate,
            cancellationToken);


        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(purchaseReturn.Id);
    }

    private static async Task<Result> MapPurchaseToPurchaseReturn(IApplicationDbContext dbContext, PurchaseReturn purchaseReturn)
    {
        var purchase = await dbContext.Purchases
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == purchaseReturn.PurchaseId);

        if (purchase is null)
        {
            return Result.Failure(Error.Failure(nameof(purchase), "Purchase not found"));
        }

        purchaseReturn.PurchaseReferenceNo = purchase.ReferenceNo;
        purchaseReturn.WarehouseId = purchase.WarehouseId;
        purchaseReturn.SupplierId = purchase.SupplierId;

        return Result.Success(); 
    }
}
