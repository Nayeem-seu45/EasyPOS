using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;
using EasyPOS.Application.Features.StockManagement.Services;

namespace EasyPOS.Application.Features.Purchases.Commands;

public record DeletePurchaseCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Purchase;
}

internal sealed class DeletePurchaseCommandHandler(
    IApplicationDbContext dbContext,
    ISupplierService supplierService,
    IStockService stockService)
    : ICommandHandler<DeletePurchaseCommand>
{
    public async Task<Result> Handle(DeletePurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = await dbContext.Purchases
            .Include(p => p.PurchaseDetails)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (purchase is null) return Result.Failure(Error.NotFound(nameof(purchase), ErrorMessages.EntityNotFound));

        // Reverse stock adjustments
        foreach (var detail in purchase.PurchaseDetails)
        {
            await stockService.AdjustStockOnPurchaseAsync(detail.ProductId, purchase.WarehouseId, -detail.Quantity, detail.NetUnitCost, isAddition: false);
        }

        // Adjust supplier financials by reversing due amount
        await supplierService.AdjustSupplierBalance(
            purchase.SupplierId,
            purchase.DueAmount, 
            PurchaseTransactionType.PurchaseDelete, 
            cancellationToken);

        dbContext.Purchases.Remove(purchase);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
