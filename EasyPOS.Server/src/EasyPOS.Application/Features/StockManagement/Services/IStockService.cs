namespace EasyPOS.Application.Features.StockManagement.Services;

internal interface IStockService
{
    Task<Result> AdjustStockOnPurchaseAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        bool isAddition = false,
        CancellationToken cancellation = default);
    Task<Result> AdjustStockOnSaleAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        bool isAddition = false,
        CancellationToken cancellationToken = default);
}
