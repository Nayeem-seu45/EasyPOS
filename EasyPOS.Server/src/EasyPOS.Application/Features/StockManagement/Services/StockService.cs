﻿using EasyPOS.Domain.Stocks;

namespace EasyPOS.Application.Features.StockManagement.Services;

public class StockService(IApplicationDbContext dbContext) : IStockService
{
    public async Task<Result> AdjustStockOnPurchaseAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        bool isAddition = false,
        CancellationToken cancellationToken = default)
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


    public async Task<Result> AdjustStockOnSaleAsync(
         Guid productId,
         Guid warehouseId,
         decimal quantity,
         bool isAddition = false,
         CancellationToken cancellationToken = default)
    {
        // Find the stock entity for the specified product and warehouse
        var stock = await dbContext.Stocks.FirstOrDefaultAsync(s =>
            s.ProductId == productId && s.WarehouseId == warehouseId);

        // Validate the existence of stock
        if (stock == null)
        {
            return Result.Failure(Error.Failure(ErrorMessages.NotFound, "Stock not found for the specified product and warehouse."));
        }

        if (isAddition) // Sale return case
        {
            // Add the returned quantity back to the stock
            stock.Quantity += quantity;
        }
        else // Sale case
        {
            // Ensure sufficient stock is available for the sale
            if (stock.Quantity < quantity)
            {
                return Result.Failure(Error.Failure(ErrorMessages.InvalidOperation, "Insufficient stock for the sale."));
            }

            // Reduce the stock quantity
            stock.Quantity -= quantity;
        }

        return Result.Success();
    }

    public async Task<Result> AdjustStockOnProductTransferAsync(
        Guid productId,
        Guid fromWarehouseId,
        Guid toWarehouseId,
        decimal quantity,
        CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            return Result.Failure(Error.Failure(ErrorMessages.InvalidOperation, "Invalid quantity."));

        // Retrieve stock from the source warehouse
        var fromStock = await dbContext.Stocks.FirstOrDefaultAsync(s =>
            s.ProductId == productId && s.WarehouseId == fromWarehouseId, cancellationToken);

        if (fromStock == null || fromStock.Quantity < quantity)
            return Result.Failure(Error.Failure(ErrorMessages.InvalidOperation, "Insufficient stock in the source warehouse."));

        // Retrieve stock from the destination warehouse
        var toStock = await dbContext.Stocks.FirstOrDefaultAsync(s =>
            s.ProductId == productId && s.WarehouseId == toWarehouseId, cancellationToken);

        // Subtract stock from the source warehouse without altering AverageUnitCost
        fromStock.Quantity -= quantity;

        // Add stock to the destination warehouse using the same AverageUnitCost from the source
        if (toStock == null)
        {
            // Create a new stock record for the destination warehouse
            toStock = AddNewStock(productId, toWarehouseId, quantity, fromStock.UnitCost, fromStock.AverageUnitCost);
            dbContext.Stocks.Add(toStock);
        }
        else
        {
            // Update the destination warehouse stock
            toStock.Quantity += quantity;
        }

        return Result.Success();
    }

    private static Stock AddNewStock(
        Guid productId, 
        Guid toWarehouseId, 
        decimal quantity, 
        decimal unitCost, 
        decimal averageUnitCost)
    {
        return new Stock
        {
            ProductId = productId,
            WarehouseId = toWarehouseId,
            Quantity = quantity,
            UnitCost = unitCost,
            AverageUnitCost = averageUnitCost
        };
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
            stock = AddNewStock(productId, warehouseId, quantity, unitCost, unitCost);
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
