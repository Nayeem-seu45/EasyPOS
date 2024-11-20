namespace EasyPOS.Application.Features.StockManagement.Services;

internal interface IStockService
{
    Task AdjustStockOnPurchaseAsync(Guid productId, Guid warehouseId, decimal quantity, decimal unitCost, bool isAddition = false);
    Task AdjustStockOnSaleAsync(Guid productId, Guid warehouseId, decimal quantity, bool isAddition = false);
}
