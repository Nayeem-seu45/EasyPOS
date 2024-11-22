using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Application.Features.PurchaseReturns.Models;
using EasyPOS.Application.Features.StockManagement.Services;
using EasyPOS.Domain.Common.Enums;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.PurchaseReturns.Commands;

public record UpdatePurchaseReturnCommand(
    Guid Id,
    DateOnly ReturnDate,
    Guid ReturnStatusId,
    string? AttachmentUrl,
    decimal SubTotal,
    decimal? TaxRate,
    decimal? TaxAmount,
    DiscountType DiscountType,
    decimal? DiscountRate,
    decimal? DiscountAmount,
    decimal? ShippingCost,
    decimal GrandTotal,
    string? Note,
    List<PurchaseReturnDetailModel> PurchaseReturnDetails) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchaseReturn;
}

internal sealed class UpdatePurchaseReturnCommandHandler(
    IApplicationDbContext dbContext,
    IPurchaseReturnService purchaseReturnService,
    IStockService stockService)
    : ICommandHandler<UpdatePurchaseReturnCommand>
{
    public async Task<Result> Handle(UpdatePurchaseReturnCommand request, CancellationToken cancellationToken)
    {
        var purchaseReturn = await dbContext.PurchaseReturns
            .Include(pr => pr.PurchaseReturnDetails) // Include related details
            .FirstOrDefaultAsync(pr => pr.Id == request.Id, cancellationToken);

        if (purchaseReturn is null) return Result.Failure(Error.NotFound(nameof(purchaseReturn), ErrorMessages.EntityNotFound));

        // Track previous details for stock adjustment
        var previousDetails = purchaseReturn.PurchaseReturnDetails.ToList();

        request.Adapt(purchaseReturn);

        await AdjustProductStock(stockService, purchaseReturn, previousDetails);

        await purchaseReturnService.AdjustPurchaseReturnAsync(
            purchaseReturn,
            purchaseReturn.PaidAmount,
            PurchaseReturnTransactionType.PurchaseReturnUpdate,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static async Task AdjustProductStock(
        IStockService stockService, 
        PurchaseReturn? purchaseReturn, 
        List<PurchaseReturnDetail> previousDetails)
    {
        // Adjust stock for the updated details
        foreach (var updatedItem in purchaseReturn.PurchaseReturnDetails)
        {
            var previousItem = previousDetails.FirstOrDefault(d => d.ProductId == updatedItem.ProductId);

            var quantityDifference = updatedItem.ReturnedQuantity - (previousItem?.ReturnedQuantity ?? 0);

            if (quantityDifference != 0)
            {
                // Adjust stock based on the quantity difference
                await stockService.AdjustStockOnPurchaseAsync(
                    productId: updatedItem.ProductId,
                    warehouseId: purchaseReturn.WarehouseId,
                    quantity: Math.Abs(quantityDifference),
                    unitCost: updatedItem.NetUnitCost,
                    isAddition: quantityDifference < 0 // Add stock if quantity decreased
                );
            }
        }
    }
}
