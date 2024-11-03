using EasyPOS.Application.Features.ProductTransfers.Queries;
using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Application.Features.ProductTransfers.Commands;

public record UpdateProductTransferCommand(
    Guid Id,
    DateOnly TransferDate,
    string ReferenceNo,
    Guid WarehouseId,
    Guid SupplierId,
    Guid TransferStatusId,
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
    List<ProductTransferDetailModel> ProductTransferDetails) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.ProductTransfer;
}

internal sealed class UpdateProductTransferCommandHandler(
    IApplicationDbContext dbContext,
    ICommonQueryService commonQueryService)
    : ICommandHandler<UpdateProductTransferCommand>
{
    public async Task<Result> Handle(UpdateProductTransferCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ProductTransfers.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
