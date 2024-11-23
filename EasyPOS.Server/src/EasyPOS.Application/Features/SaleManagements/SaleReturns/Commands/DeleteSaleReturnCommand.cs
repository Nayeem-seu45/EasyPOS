using EasyPOS.Application.Features.StockManagement.Services;

namespace EasyPOS.Application.Features.SaleReturns.Commands;

public record DeleteSaleReturnCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SaleReturn;
}

internal sealed class DeleteSaleReturnCommandHandler(
    IApplicationDbContext dbContext,
    IStockService stockService)
    : ICommandHandler<DeleteSaleReturnCommand>
{
    public async Task<Result> Handle(DeleteSaleReturnCommand request, CancellationToken cancellationToken)
    {
        var saleReturn = await dbContext.SaleReturns
            .Include(sr => sr.SaleReturnDetails)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (saleReturn is null)
        {
            return Result.Failure(Error.NotFound(nameof(saleReturn), ErrorMessages.EntityNotFound));
        }

        // Adjust stock for all returned items before deletion
        foreach (var detail in saleReturn.SaleReturnDetails)
        {
            var stockAdjustmentResult = await stockService.AdjustStockOnSaleAsync(
                productId: detail.ProductId,
                warehouseId: saleReturn.WarehouseId,
                quantity: detail.ReturnedQuantity,
                isAddition: false, // Revert stock changes
                cancellationToken: cancellationToken
            );

            if (!stockAdjustmentResult.IsSuccess)
            {
                return Result.Failure(stockAdjustmentResult.Error);
            }
        }

        dbContext.SaleReturns.Remove(saleReturn);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

