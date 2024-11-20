using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Application.Features.Purchases.Models;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;
using EasyPOS.Application.Features.StockManagement.Services;
using EasyPOS.Domain.Common.Enums;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.Purchases.Commands;

public record UpdatePurchaseCommand(
    Guid Id,
    DateOnly PurchaseDate,
    string ReferenceNo,
    Guid WarehouseId,
    Guid SupplierId,
    Guid PurchaseStatusId,
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
    List<PurchaseDetailModel> PurchaseDetails) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Purchase;
}

internal sealed class UpdatePurchaseCommandHandler(
    IApplicationDbContext dbContext,
    ISupplierService supplierFinancialService,
    IPurchaseService purchaseService,
    IStockService stockService)
    : ICommandHandler<UpdatePurchaseCommand>
{
    public async Task<Result> Handle(UpdatePurchaseCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the existing purchase
        var purchase = await dbContext.Purchases.FindAsync(request.Id, cancellationToken);
        if (purchase is null) return Result.Failure(Error.NotFound(nameof(purchase), ErrorMessages.EntityNotFound));

        var oldDueAmount = purchase.DueAmount;

        // Update the entity with new values
        request.Adapt(purchase);

        // Recalculate DueAmount and PaymentStatusId
        purchase.DueAmount = purchase.GrandTotal - purchase.PaidAmount;
        purchase.PaymentStatusId = await purchaseService.GetPurchasePaymentId(purchase, cancellationToken);

        // Update Stock
        foreach (var detail in purchase.PurchaseDetails) 
        { 
            await stockService.AdjustStockAsync(detail.ProductId, purchase.WarehouseId, detail.Quantity, detail.NetUnitCost, true); 
        }

        // Adjust supplier financials
        var dueAmountDifference = purchase.DueAmount - oldDueAmount;
        await supplierFinancialService.AdjustSupplierBalance(
           purchase.SupplierId,
           dueAmountDifference,
           PurchaseTransactionType.PurchaseUpdate,
           cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
