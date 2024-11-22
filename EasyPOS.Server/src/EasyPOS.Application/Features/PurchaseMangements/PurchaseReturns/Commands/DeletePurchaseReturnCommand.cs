using EasyPOS.Application.Features.StockManagement.Services;

namespace EasyPOS.Application.Features.PurchaseReturns.Commands;

public record DeletePurchaseReturnCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchaseReturn;
}

internal sealed class DeletePurchaseReturnCommandHandler(
    IApplicationDbContext dbContext,
    IStockService stockService)
    : ICommandHandler<DeletePurchaseReturnCommand>
{
    public async Task<Result> Handle(DeletePurchaseReturnCommand request, CancellationToken cancellationToken)
    {
        // Load the purchase return and its details
        var purchaseReturn = await dbContext.PurchaseReturns
            .Include(pr => pr.PurchaseReturnDetails)
            .FirstOrDefaultAsync(pr => pr.Id == request.Id, cancellationToken);

        if (purchaseReturn is null)
            return Result.Failure(Error.NotFound(nameof(purchaseReturn), ErrorMessages.EntityNotFound));

        // Restore stock for each item in the purchase return
        foreach (var detail in purchaseReturn.PurchaseReturnDetails)
        {
            await stockService.AdjustStockOnPurchaseAsync(
                productId: detail.ProductId,
                warehouseId: purchaseReturn.WarehouseId,
                quantity: detail.ReturnedQuantity,
                unitCost: detail.NetUnitCost,
                isAddition: true // Re-add stock for deleted purchase return
            );
        }

        dbContext.PurchaseReturns.Remove(purchaseReturn);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
