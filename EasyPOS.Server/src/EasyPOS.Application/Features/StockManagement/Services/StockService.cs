using EasyPOS.Domain.Stocks;

namespace EasyPOS.Application.Features.StockManagement.Services;

public class StockService(IApplicationDbContext dbContext) : IStockService
{
    public async Task AdjustStockAsync(Guid productId, Guid warehouseId, decimal quantity, decimal unitCost, bool isAddition = false)
    {
        if (unitCost <= 0)
            throw new InvalidOperationException("Invalid unit cost.");

        var stock = await dbContext.Stocks.FirstOrDefaultAsync(s =>
            s.ProductId == productId && s.WarehouseId == warehouseId);

        if (isAddition)
        {
            if (stock is null)
            {
                stock = new Stock
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    Quantity = quantity,
                    UnitCost = unitCost,
                    AverageUnitCost = unitCost
                };
                dbContext.Stocks.Add(stock);
            }
            else
            {
                stock.AverageUnitCost = CalculateWeightedAverage(stock.Quantity, stock.AverageUnitCost, quantity, unitCost);
                stock.Quantity += quantity;
            }
        }
        else
        {
            if (stock == null || stock.Quantity < quantity)
                throw new InvalidOperationException("Insufficient stock.");

            stock.Quantity -= quantity;
            // Optionally adjust AverageUnitCost here if needed.
        }
    }
    public static decimal CalculateWeightedAverage(decimal oldQuantity, decimal oldCost, decimal newQuantity, decimal newCost)
    {
        return ((oldQuantity * oldCost) + (newQuantity * newCost)) / (oldQuantity + newQuantity);
    }

}
