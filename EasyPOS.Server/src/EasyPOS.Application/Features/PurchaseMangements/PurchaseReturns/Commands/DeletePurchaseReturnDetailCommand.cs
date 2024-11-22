using EasyPOS.Application.Features.StockManagement.Services;

namespace EasyPOS.Application.Features.PurchaseReturns.Commands;

public record DeletePurchaseReturnDetailCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchaseReturn;
}

internal sealed class DeletePurchaseReturnDetailCommandHandler(
    IApplicationDbContext dbContext,
    IStockService stockService)
    : ICommandHandler<DeletePurchaseReturnDetailCommand>
{
    public async Task<Result> Handle(DeletePurchaseReturnDetailCommand request, CancellationToken cancellationToken)
    {
        var purchaseReturnDetail = await dbContext.PurchaseReturnDetails
            .Include(x => x.PurchaseReturn)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (purchaseReturnDetail is null) return Result.Failure(Error.NotFound(nameof(purchaseReturnDetail), ErrorMessages.EntityNotFound));

        dbContext.PurchaseReturnDetails.Remove(purchaseReturnDetail);

        await stockService.AdjustStockOnPurchaseAsync(
            productId: purchaseReturnDetail.ProductId,
            warehouseId: purchaseReturnDetail.PurchaseReturn.WarehouseId,
            quantity: purchaseReturnDetail.ReturnedQuantity,
            unitCost: purchaseReturnDetail.NetUnitCost,
            isAddition: true // Re-add stock for removed items
        );

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
