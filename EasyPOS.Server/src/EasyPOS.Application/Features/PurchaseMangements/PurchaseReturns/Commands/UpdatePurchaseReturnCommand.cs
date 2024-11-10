using EasyPOS.Application.Features.PurchaseReturns.Models;
using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Application.Features.PurchaseReturns.Commands;

public record UpdatePurchaseReturnCommand(
    Guid Id,
    DateOnly PurchaseReturnDate,
    string ReferenceNo,
    Guid WarehouseId,
    Guid SupplierId,
    Guid PurchaseReturnStatusId,
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
    ICommonQueryService commonQueryService)
    : ICommandHandler<UpdatePurchaseReturnCommand>
{
    public async Task<Result> Handle(UpdatePurchaseReturnCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.PurchaseReturns.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
