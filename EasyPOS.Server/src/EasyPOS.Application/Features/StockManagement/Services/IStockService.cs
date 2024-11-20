namespace EasyPOS.Application.Features.StockManagement.Services;

internal interface IStockService
{
    Task AdjustStockAsync(Guid productId, Guid warehouseId, decimal quantity, decimal unitCost, bool isAddition = false);
}
