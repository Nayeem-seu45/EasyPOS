using EasyPOS.Application.Features.StockManagement.Services;

namespace EasyPOS.Application.Features.Purchases.Commands;

public record DeletePurchaseDetailCommand(Guid Id) : ICacheInvalidatorCommand
{
    [JsonIgnore]
    public string CacheKey => CacheKeys.Purchase;
}

internal sealed class DeletePurchaseDetailCommandHandler(
    IApplicationDbContext dbContext,
    IStockService stockService)
    : ICommandHandler<DeletePurchaseDetailCommand>
{
    public async Task<Result> Handle(DeletePurchaseDetailCommand request, CancellationToken cancellationToken)
    {
        var purchaseDetail = await dbContext.PurchaseDetails
            .Include(x => x.Purchase)
            .FirstOrDefaultAsync(x  => x.Id == request.Id, cancellationToken);

        if (purchaseDetail is null) return Result.Failure(Error.NotFound(nameof(purchaseDetail), ErrorMessages.EntityNotFound));

        dbContext.PurchaseDetails.Remove(purchaseDetail);

        await stockService.AdjustStockOnPurchaseAsync(
            productId: purchaseDetail.ProductId,
            warehouseId: purchaseDetail.Purchase.WarehouseId,
            quantity: purchaseDetail.Quantity,
            unitCost: purchaseDetail.NetUnitCost,
            isAddition: false 
        );

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
