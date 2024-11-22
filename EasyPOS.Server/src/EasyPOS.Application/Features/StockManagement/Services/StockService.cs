﻿using EasyPOS.Domain.Stocks;

namespace EasyPOS.Application.Features.StockManagement.Services;

public class StockService(IApplicationDbContext dbContext) : IStockService
{
    public async Task<Result> AdjustStockOnPurchaseAsync(
        Guid productId, 
        Guid warehouseId, 
        decimal quantity,
        decimal unitCost, 
        bool isAddition = false)
    {
        if (unitCost <= 0)
            return Result.Failure(Error.Failure(ErrorMessages.InvalidOperation, "Invalid unit cost."));


        var stock = await dbContext.Stocks.FirstOrDefaultAsync(s =>
            s.ProductId == productId && s.WarehouseId == warehouseId);

        Result result;

        if (isAddition)
        {
           result = AddToStock(stock, productId, warehouseId, quantity, unitCost);
        }
        else
        {
           result = SubtractFromStock(stock, quantity, unitCost);
        }
        return result;
    }


    // Handles sale and sale adjustments
    public async Task<Result> AdjustStockOnSaleAsync(
        Guid productId, 
        Guid warehouseId, 
        decimal quantity, 
        bool isAddition = false)
    {
        var stock = await dbContext.Stocks.FirstOrDefaultAsync(s =>
            s.ProductId == productId && s.WarehouseId == warehouseId);

        if (isAddition)
        {
            if (stock is null)
                return Result.Failure(Error.Failure(ErrorMessages.NotFound, "Cannot return items to a stock that doesn't exist."));

            stock.Quantity += quantity;
        }
        else
        {
            if (stock == null || stock.Quantity < Math.Abs(quantity))
                return Result.Failure(Error.Failure(ErrorMessages.InvalidOperation, "Insufficient stock."));

            stock.Quantity -= Math.Abs(quantity);
        }
        return Result.Success();
    }


    private Result AddToStock(
        Stock stock, 
        Guid productId, 
        Guid warehouseId, 
        decimal quantity, 
        decimal unitCost)
    {
        if (stock is null)
        {
            stock = new Stock
            {
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
            stock.AverageUnitCost = CalculateWeightedAverage(
                stock.Quantity, 
                stock.AverageUnitCost, 
                quantity, 
                unitCost);
            stock.Quantity += quantity;
        }
        return Result.Success();
    }

    private static Result SubtractFromStock(Stock stock, decimal quantity, decimal unitCost)
    {
        if (stock == null || stock.Quantity < Math.Abs(quantity))
            return Result.Failure(Error.Failure(ErrorMessages.InvalidOperation, "Insufficient stock."));


        var oldStockQty = stock.Quantity;

        stock.Quantity -= Math.Abs(quantity);

        AdjustAverageUnitCost(stock, oldStockQty, quantity, unitCost);

        return Result.Success();
    }


    private static void AdjustAverageUnitCost(
        Stock stock, 
        decimal oldStockQty, 
        decimal quantity, 
        decimal unitCost)
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

    private static decimal CalculateWeightedAverage(
        decimal oldQuantity, 
        decimal oldCost, 
        decimal newQuantity, 
        decimal newCost)
    {
        return ((oldQuantity * oldCost) + (newQuantity * newCost)) / (oldQuantity + newQuantity);
    }
}
