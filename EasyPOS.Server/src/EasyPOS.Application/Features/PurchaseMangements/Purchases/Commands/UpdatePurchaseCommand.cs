using EasyPOS.Application.Features.Purchases.Models;
using EasyPOS.Application.Features.Purchases.Shared;
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
    ICommonQueryService commonQueryService)
    : ICommandHandler<UpdatePurchaseCommand>
{
    public async Task<Result> Handle(UpdatePurchaseCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the existing purchase
        var entity = await dbContext.Purchases.FindAsync(request.Id, cancellationToken);
        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        // Store old values for comparison
        var oldGrandTotal = entity.GrandTotal;
        var oldPaidAmount = entity.PaidAmount;
        var oldDueAmount = entity.DueAmount;

        // Update the entity with new values
        request.Adapt(entity);

        // Recalculate DueAmount and PaymentStatusId
        entity.DueAmount = entity.GrandTotal - entity.PaidAmount;
        entity.PaymentStatusId = await PurchaseSharedService.GetPurchasePaymentId(commonQueryService, entity);

        // Update Supplier's financial records based on the changes in amounts
        await UpdateSupplierFinancials(entity, oldGrandTotal, oldPaidAmount, oldDueAmount);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task UpdateSupplierFinancials(
        Purchase purchase, 
        decimal oldGrandTotal, 
        decimal oldPaidAmount,
        decimal oldDueAmount)
    {
        // Retrieve the supplier
        var supplier = await dbContext.Suppliers.FindAsync(purchase.SupplierId);
        if (supplier is null) return;

        // Calculate differences
        var grandTotalDifference = purchase.GrandTotal - oldGrandTotal;
        var paidAmountDifference = purchase.PaidAmount - oldPaidAmount;
        var dueAmountDifference = purchase.DueAmount - oldDueAmount;

        // Update supplier's financials
        supplier.TotalDueAmount += dueAmountDifference;
        supplier.TotalPaidAmount += paidAmountDifference;

        // Recalculate OutstandingBalance
        supplier.OutstandingBalance = supplier.CalculateOutstandingBalance();
    }
}
