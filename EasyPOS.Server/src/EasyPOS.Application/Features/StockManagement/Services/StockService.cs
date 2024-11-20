using EasyPOS.Domain.Stocks;

namespace EasyPOS.Application.Features.StockManagement.Services;

public class StockService(IApplicationDbContext dbContext) : IStockService
{
    public async Task AdjustStockOnPurchaseAsync(Guid productId, Guid warehouseId, decimal quantity, decimal unitCost, bool isAddition = false)
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
            if (stock == null || stock.Quantity < Math.Abs(quantity))
                throw new InvalidOperationException("Insufficient stock.");

            // Store the old quantity for calculations
            var oldStockQty = stock.Quantity;

            // Adjust stock quantity
            stock.Quantity -= Math.Abs(quantity);

            // Adjust AverageUnitCost
            AdjustAverageUnitCost(stock, oldStockQty, quantity, unitCost);
        }
    }

    // Handles sale and sale return adjustments
    public async Task AdjustStockOnSaleAsync(Guid productId, Guid warehouseId, decimal quantity, bool isAddition = false)
    {
        var stock = await dbContext.Stocks.FirstOrDefaultAsync(s =>
            s.ProductId == productId && s.WarehouseId == warehouseId);

        if (isAddition)
        {
            if (stock is null)
                throw new InvalidOperationException("Cannot return items to a stock that doesn't exist.");

            stock.Quantity += quantity;
        }
        else
        {
            if (stock == null || stock.Quantity < Math.Abs(quantity))
                throw new InvalidOperationException("Insufficient stock.");

            stock.Quantity -= Math.Abs(quantity);
        }
    }

    private static void AdjustAverageUnitCost(Stock stock, decimal oldStockQty, decimal quantity, decimal unitCost)
    {
        // Calculate the cost of the removed items
        var netCostRemoved = Math.Abs(quantity) * unitCost;

        // Calculate the new total stock value
        var totalStockValue = (oldStockQty * stock.AverageUnitCost) - netCostRemoved;

        // Recalculate the AverageUnitCost
        if (stock.Quantity > 0)
        {
            stock.AverageUnitCost = totalStockValue / stock.Quantity;
        }
        else
        {
            // Reset to 0 if no stock remains
            stock.AverageUnitCost = 0;
        }
    }

    private static decimal CalculateWeightedAverage(decimal oldQuantity, decimal oldCost, decimal newQuantity, decimal newCost)
    {
        return ((oldQuantity * oldCost) + (newQuantity * newCost)) / (oldQuantity + newQuantity);
    }
}
