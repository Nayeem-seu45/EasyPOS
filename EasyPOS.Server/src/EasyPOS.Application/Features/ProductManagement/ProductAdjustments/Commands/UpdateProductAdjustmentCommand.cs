using EasyPOS.Application.Features.ProductManagement.ProductAdjustments.Queries;

namespace EasyPOS.Application.Features.ProductManagement.ProductAdjustments.Commands;

public record UpdateProductAdjustmentCommand(
    Guid Id,
    Guid WarehouseId, 
    string? AttachmentUrl, 
    string? Note, 
    DateTime AdjDate,
    List<ProductAdjustmentDetailModel> ProductAdjustmentDetails
    ) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.ProductAdjustment;
}

internal sealed class UpdateProductAdjustmentCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateProductAdjustmentCommand>
{
    public async Task<Result> Handle(UpdateProductAdjustmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ProductAdjustments.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        entity.TotalQuantity = entity.ProductAdjustmentDetails.Count;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
