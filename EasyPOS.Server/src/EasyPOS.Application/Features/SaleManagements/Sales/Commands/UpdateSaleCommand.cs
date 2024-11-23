using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Application.Features.Sales.Models;
using EasyPOS.Application.Features.Stakeholders.Customers.Services;
using EasyPOS.Application.Features.StockManagement.Services;
using EasyPOS.Domain.Sales;

namespace EasyPOS.Application.Features.Sales.Commands;

public record UpdateSaleCommand : UpsertSaleModel, ICacheInvalidatorCommand
{
    [JsonIgnore]
    public string CacheKey => CacheKeys.Sale;
}

internal sealed class UpdateSaleCommandHandler(
    IApplicationDbContext dbContext,
    ISaleService saleService,
    ICustomerService customerService,
    IStockService stockService)
    : ICommandHandler<UpdateSaleCommand>
{
    public async Task<Result> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the existing sale entity
        var entity = await dbContext.Sales
            .Include(x => x.SaleDetails)
            .Include(s => s.SalePayments) // Load SalePayments to verify payments cannot be modified
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));
        }

        // Retrieve the original sale details
        var originalDetails = entity.SaleDetails.ToList();

        // Calculate the original due and paid amounts for adjusting the customer's record
        var previousDueAmount = entity.DueAmount;
        var previousPaidAmount = entity.PaidAmount;

        // Adapt the non-payment properties from the request
        request.Adapt(entity);

        // Use SaleService to adjust sale due and payment status
        await saleService.AdjustSaleAsync(SaleTransactionType.SaleUpdate, entity, 0, cancellationToken);

        // Handle stock adjustments
        var stockAdjustmentResult = await HandleStockAdjustmentsAsync(
            originalDetails,
            request.SaleDetails,
            entity.WarehouseId,
            cancellationToken);

        if (!stockAdjustmentResult.IsSuccess)
        {
            return stockAdjustmentResult;
        }

        // Update the customer's due and paid amounts via CustomerService
        var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == entity.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result.Failure(Error.Failure(nameof(customer), "Customer not found."));
        }

        // Adjust customer totals by removing the old amounts and adding the new amounts
        customerService.AdjustCustomerOnSale(
            SaleTransactionType.SaleUpdate,
            customer,
            entity.DueAmount - previousDueAmount
        );

        // Save all changes
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result> HandleStockAdjustmentsAsync(
        List<SaleDetail> originalDetails,
        List<SaleDetailModel> updatedDetails,
        Guid warehouseId,
        CancellationToken cancellationToken = default)
    {
        var stockAdjustments = new List<Result>();

        // Handle removed or modified products
        foreach (var originalDetail in originalDetails)
        {
            // Check if the product exists in the updated request
            var updatedDetail = updatedDetails.FirstOrDefault(d => d.ProductId == originalDetail.ProductId);

            if (updatedDetail == null)
            {
                // Product was removed in the update - restore the stock
                var restoreStockResult = await stockService.AdjustStockOnSaleAsync(
                    originalDetail.ProductId,
                    warehouseId,
                    originalDetail.Quantity,
                    isAddition: true, // Restore stock for removed products
                    cancellationToken
                );

                stockAdjustments.Add(restoreStockResult);
            }
            else if (updatedDetail.Quantity != originalDetail.Quantity)
            {
                // Quantity changed for the product - adjust the stock difference
                var quantityDifference = originalDetail.Quantity - updatedDetail.Quantity;

                var adjustStockResult = await stockService.AdjustStockOnSaleAsync(
                    originalDetail.ProductId,
                    warehouseId,
                    Math.Abs(quantityDifference),
                    isAddition: quantityDifference > 0, // Restore stock if reducing quantity
                    cancellationToken
                );

                stockAdjustments.Add(adjustStockResult);
            }
        }

        // Handle newly added products
        foreach (var updatedDetail in updatedDetails)
        {
            if (!originalDetails.Any(d => d.ProductId == updatedDetail.ProductId))
            {
                // Product was newly added - reduce stock
                var reduceStockResult = await stockService.AdjustStockOnSaleAsync(
                    updatedDetail.ProductId,
                    warehouseId,
                    updatedDetail.Quantity,
                    isAddition: false, // Reduce stock for newly added products
                    cancellationToken
                );

                stockAdjustments.Add(reduceStockResult);
            }
        }

        // Check for any stock adjustment failures
        if (stockAdjustments.Any(r => !r.IsSuccess))
        {
            return Result.Failure(stockAdjustments.First(r => !r.IsSuccess).Error);
        }

        return Result.Success();
    }
}
