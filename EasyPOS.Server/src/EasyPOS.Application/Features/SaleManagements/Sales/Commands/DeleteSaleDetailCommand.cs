using EasyPOS.Application.Features.StockManagement.Services;

namespace EasyPOS.Application.Features.Sales.Commands;

public record DeleteSaleDetailCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Sale;
}

internal sealed class DeleteSaleDetailCommandHandler(
    IApplicationDbContext dbContext,
    IStockService stockService)
    : ICommandHandler<DeleteSaleDetailCommand>
{
    public async Task<Result> Handle(DeleteSaleDetailCommand request, CancellationToken cancellationToken)
    {
        var saleDetail = await dbContext.SaleDetails
            .Include(x => x.Sale)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (saleDetail is null) return Result.Failure(Error.NotFound(nameof(saleDetail), ErrorMessages.EntityNotFound));

        // Adjust stock to restore the quantity for the deleted SaleDetail
        var stockAdjustmentResult = await stockService.AdjustStockOnSaleAsync(
            productId: saleDetail.ProductId,
            warehouseId: saleDetail.Sale.WarehouseId,
            quantity: saleDetail.Quantity,
            isAddition: true // Restore stock since the item is being removed
        );

        if (!stockAdjustmentResult.IsSuccess)
        {
            return stockAdjustmentResult; // Return failure if stock adjustment fails
        }

        dbContext.SaleDetails.Remove(saleDetail);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
