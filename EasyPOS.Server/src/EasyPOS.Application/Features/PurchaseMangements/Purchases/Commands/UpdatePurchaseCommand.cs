using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Application.Features.Purchases.Models;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;
using EasyPOS.Domain.Common.Enums;

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
    IPurchaseService purchaseService)
    : ICommandHandler<UpdatePurchaseCommand>
{
    public async Task<Result> Handle(UpdatePurchaseCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the existing purchase
        var entity = await dbContext.Purchases.FindAsync(request.Id, cancellationToken);
        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        var oldDueAmount = entity.DueAmount;

        // Update the entity with new values
        request.Adapt(entity);

        // Recalculate DueAmount and PaymentStatusId
        entity.DueAmount = entity.GrandTotal - entity.PaidAmount;
        entity.PaymentStatusId = await purchaseService.GetPurchasePaymentId(entity, cancellationToken);

        // Adjust supplier financials
        var dueAmountDifference = entity.DueAmount - oldDueAmount;
        await supplierFinancialService.AdjustSupplierBalance(
           entity.SupplierId,
           dueAmountDifference,
           PurchaseTransactionType.PurchaseUpdate,
           cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
