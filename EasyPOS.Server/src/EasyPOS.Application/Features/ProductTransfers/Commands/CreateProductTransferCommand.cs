using EasyPOS.Application.Features.ProductTransfers.Queries;
using EasyPOS.Domain.Common.Enums;
using EasyPOS.Domain.ProductTransfers;

namespace EasyPOS.Application.Features.ProductTransfers.Commands;

public record CreateProductTransferCommand(
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
    List<ProductTransferDetailModel> ProductTransferDetails
    ) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.ProductTransfer;
}

internal sealed class CreateProductTransferCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<CreateProductTransferCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductTransferCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<ProductTransfer>();
        dbContext.ProductTransfers.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}
