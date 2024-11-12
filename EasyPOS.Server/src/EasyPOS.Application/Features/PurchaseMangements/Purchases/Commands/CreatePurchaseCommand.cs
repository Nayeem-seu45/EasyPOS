using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.Purchases.Models;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Models;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;
using EasyPOS.Domain.Common.Enums;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.Purchases.Commands;

public record CreatePurchaseCommand(
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
    List<PurchaseDetailModel> PurchaseDetails
    ) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Purchase;
}

internal sealed class CreatePurchaseCommandHandler(
    IApplicationDbContext dbContext,
    ISupplierFinancialService supplierFinancialService,
    IPurchaseService purchaseService)
    : ICommandHandler<CreatePurchaseCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<Purchase>();
        entity.DueAmount = entity.GrandTotal;
        entity.PurchaseStatusId = await purchaseService.GetPurchasePaymentId(entity) ?? Guid.Empty;
        dbContext.Purchases.Add(entity);

        // Adjust supplier financials
        await supplierFinancialService.AdjustSupplierBalance(
            entity.SupplierId, 
            entity.DueAmount, 
            PurchaseTransactionType.Purchase, 
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}
