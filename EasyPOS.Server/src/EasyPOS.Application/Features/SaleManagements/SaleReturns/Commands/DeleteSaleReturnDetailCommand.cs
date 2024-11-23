using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using EasyPOS.Application.Features.StockManagement.Services;

namespace EasyPOS.Application.Features.SaleReturns.Commands;

public record DeleteSaleReturnDetailCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SaleReturn;
}

internal sealed class DeleteSaleReturnDetailCommandHandler(
    IApplicationDbContext dbContext,
    IStockService stockService)
    : ICommandHandler<DeleteSaleReturnDetailCommand>
{
    public async Task<Result> Handle(DeleteSaleReturnDetailCommand request, CancellationToken cancellationToken)
    {
        var saleReturnDetail = await dbContext.SaleReturnDetails.FindAsync(request.Id, cancellationToken);

        if (saleReturnDetail is null) return Result.Failure(Error.NotFound(nameof(saleReturnDetail), ErrorMessages.EntityNotFound));

        // Adjust stock for the deleted detail
        var stockAdjustmentResult = await stockService.AdjustStockOnSaleAsync(
            productId: saleReturnDetail.ProductId,
            warehouseId: saleReturnDetail.SaleReturn.WarehouseId,
            quantity: saleReturnDetail.ReturnedQuantity,
            isAddition: false, // Revert stock changes
            cancellationToken: cancellationToken
        );

        if (!stockAdjustmentResult.IsSuccess)
        {
            return Result.Failure(stockAdjustmentResult.Error);
        }

        dbContext.SaleReturnDetails.Remove(saleReturnDetail);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
