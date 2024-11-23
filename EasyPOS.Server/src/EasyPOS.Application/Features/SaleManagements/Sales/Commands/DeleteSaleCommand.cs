using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Application.Features.Stakeholders.Customers.Services;
using EasyPOS.Application.Features.StockManagement.Services;
using EasyPOS.Domain.Sales;

namespace EasyPOS.Application.Features.Sales.Commands;

public record DeleteSaleCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Sale;
}

internal sealed class DeleteSaleCommandHandler(
    IApplicationDbContext dbContext,
    IStockService stockService,
    ICustomerService customerService)
    : ICommandHandler<DeleteSaleCommand>

{
    public async Task<Result> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        // Find the sale with its details
        var sale = await dbContext.Sales
            .Include(s => s.SaleDetails) // Load sale details for stock adjustments
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (sale is null)
        {
            return Result.Failure(Error.NotFound(nameof(sale), ErrorMessages.EntityNotFound));
        }

        // Adjust stock based on sale details
        var stockAdjustmentResult = await HandleStockAdjustmentsAsync(sale.SaleDetails.ToList(), sale.WarehouseId, cancellationToken);
        if (!stockAdjustmentResult.IsSuccess)
        {
            return stockAdjustmentResult; // Return failure if stock adjustment fails
        }

        // Adjust customer financials based on the sale
        var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == sale.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result.Failure(Error.NotFound(nameof(customer), "Customer not found."));
        }

        customerService.AdjustCustomerOnSale(
            SaleTransactionType.SaleDelete,
            customer,
            dueAmount: sale.DueAmount,
            paidAmount: sale.PaidAmount
        );

        // Remove the sale record
        dbContext.Sales.Remove(sale);

        // Save changes to the database
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result> HandleStockAdjustmentsAsync(
        List<SaleDetail> saleDetails,
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var stockAdjustments = new List<Result>();

        foreach (var detail in saleDetails)
        {
            // Restore stock for each product in the sale
            var result = await stockService.AdjustStockOnSaleAsync(
                detail.ProductId,
                warehouseId,
                detail.Quantity,
                isAddition: true // Restore stock
            );

            stockAdjustments.Add(result);
        }

        // Check if any stock adjustment failed
        if (stockAdjustments.Any(r => !r.IsSuccess))
        {
            return Result.Failure(stockAdjustments.First(r => !r.IsSuccess).Error);
        }

        return Result.Success();
    }

}
