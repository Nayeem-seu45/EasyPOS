using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Application.Features.SaleReturns.Models;
using EasyPOS.Application.Features.StockManagement.Services;
using EasyPOS.Domain.Sales;

namespace EasyPOS.Application.Features.SaleReturns.Commands;

public record UpdateSaleReturnCommand : UpsertSaleReturnModel, ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SaleReturn;
}

internal sealed class UpdateSaleReturnCommandHandler(
    IApplicationDbContext dbContext,
    ISaleReturnService saleReturnService,
    IStockService stockService)
    : ICommandHandler<UpdateSaleReturnCommand>
{
    public async Task<Result> Handle(UpdateSaleReturnCommand request, CancellationToken cancellationToken)
    {
        var saleReturn = await dbContext.SaleReturns
            .Include(sr => sr.SaleReturnDetails)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (saleReturn is null)
            return Result.Failure(Error.NotFound(nameof(saleReturn), ErrorMessages.EntityNotFound));

        // Adjust stock for removed or updated items
        var stockAdjustmentResult = await AdjustStockForSaleReturnItemsAsync(
            saleReturn.SaleReturnDetails,
            saleReturn.WarehouseId,
            isAddition: false,
            cancellationToken
        );

        if (!stockAdjustmentResult.IsSuccess)
            return Result.Failure(stockAdjustmentResult.Error);

        // Update SaleReturn with new details
        request.Adapt(saleReturn);

        // Adjust stock for new or updated items
        stockAdjustmentResult = await AdjustStockForSaleReturnItemsAsync(
            saleReturn.SaleReturnDetails,
            saleReturn.WarehouseId,
            isAddition: true,
            cancellationToken
        );

        if (!stockAdjustmentResult.IsSuccess)
            return Result.Failure(stockAdjustmentResult.Error);

        await saleReturnService.AdjustSaleReturnAsync(
            SaleReturnTransactionType.SaleReturnUpdate,
            saleReturn,
            saleReturn.PaidAmount,
            cancellationToken
        );

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    // Local method to adjust stock for sale return items
    private async Task<Result> AdjustStockForSaleReturnItemsAsync(
        IEnumerable<SaleReturnDetail> saleReturnDetails,
        Guid warehouseId,
        bool isAddition,
        CancellationToken cancellationToken)
    {
        foreach (var detail in saleReturnDetails)
        {
            var stockAdjustmentResult = await stockService.AdjustStockOnSaleAsync(
                productId: detail.ProductId,
                warehouseId: warehouseId,
                quantity: detail.ReturnedQuantity,
                isAddition: isAddition,
                cancellationToken: cancellationToken
            );

            if (!stockAdjustmentResult.IsSuccess)
                return Result.Failure(stockAdjustmentResult.Error);
        }

        return Result.Success();
    }
}

