using EasyPOS.Application.Features.PurchaseReturns.Models;
using EasyPOS.Domain.Common.Enums;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.PurchaseReturns.Commands;

public record CreatePurchaseReturnCommand(
    DateOnly ReturnDate,
    string ReferenceNo,
    Guid PurchaseId,
    //Guid SupplierId,
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
    IApplicationDbContext dbContext)
    : ICommandHandler<CreatePurchaseReturnCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePurchaseReturnCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<PurchaseReturn>();
        dbContext.PurchaseReturns.Add(entity);
        entity.ReferenceNo = DateTime.Now.ToString("yyyyMMddhhmmffff");
        entity.PurchaseReferenceNo = DateTime.Now.ToString("yyyyMMddhhmmffff");
        entity.WarehouseId = entity.PurchaseId;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}
