namespace EasyPOS.Application.Features.ProductManagement.ProductAdjustments.Commands;

public record DeleteProductAdjustmentDetailCommand(Guid Id): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.ProductAdjustment;
}

internal sealed class DeleteProductAdjustmentDetailCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteProductAdjustmentDetailCommand>

{
    public async Task<Result> Handle(DeleteProductAdjustmentDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ProductAdjustmentDetails
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.ProductAdjustmentDetails.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}
