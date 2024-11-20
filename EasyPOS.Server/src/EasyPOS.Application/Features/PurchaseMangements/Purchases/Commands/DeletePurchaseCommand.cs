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
        var purchase = await dbContext.Purchases.FindAsync(request.Id, cancellationToken);

        if (purchase is null) return Result.Failure(Error.NotFound(nameof(purchase), ErrorMessages.EntityNotFound));

        dbContext.Purchases.Remove(purchase);

        // Update Stock
        foreach (var detail in purchase.PurchaseDetails)
        {
            await stockService.AdjustStockAsync(detail.ProductId, purchase.WarehouseId, detail.Quantity, detail.NetUnitCost, true);
        }

        // Adjust supplier financials by reversing due amount
        await supplierService.AdjustSupplierBalance(
            purchase.SupplierId,
            purchase.DueAmount, 
            PurchaseTransactionType.PurchaseDelete, 
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
