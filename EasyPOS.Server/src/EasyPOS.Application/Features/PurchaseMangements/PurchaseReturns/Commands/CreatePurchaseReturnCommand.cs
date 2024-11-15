using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Application.Features.PurchaseReturns.Models;
using EasyPOS.Domain.Common.Enums;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.PurchaseReturns.Commands;

public record CreatePurchaseReturnCommand(
    DateOnly ReturnDate,
    Guid PurchaseId,
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
    List<PurchaseReturnDetailModel> PurchaseReturnDetails
    ) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.PurchaseReturn;
}

internal sealed class CreatePurchaseReturnCommandHandler(
    IApplicationDbContext dbContext,
    IPurchaseReturnService purchaseReturnService)
    : ICommandHandler<CreatePurchaseReturnCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePurchaseReturnCommand request, CancellationToken cancellationToken)
    {
        var purchaseReturn = request.Adapt<PurchaseReturn>();
        dbContext.PurchaseReturns.Add(purchaseReturn);

        purchaseReturn.ReferenceNo = "PR-" + DateTime.Now.ToString("yyyyMMddhhmmffff");

        var purchase = await dbContext.Purchases
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == purchaseReturn.PurchaseId);

        if (purchase is null)
        {
            return Result.Failure<Guid>(Error.Failure(nameof(purchase), "PurchaseCreate not found"));
        }

        purchaseReturn.PurchaseReferenceNo = purchase.ReferenceNo;
        purchaseReturn.WarehouseId = purchase.WarehouseId;
        purchaseReturn.SupplierId = purchase.SupplierId;


        await purchaseReturnService.AdjustPurchaseReturnAsync(
            purchaseReturn, 
            0, 
            PurchaseReturnTransactionType.PurchaseReturnCreate, 
            cancellationToken);


        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(purchaseReturn.Id);
    }
}
