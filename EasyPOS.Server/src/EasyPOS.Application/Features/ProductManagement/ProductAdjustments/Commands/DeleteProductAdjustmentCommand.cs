namespace EasyPOS.Application.Features.ProductManagement.ProductAdjustments.Commands;

public record DeleteProductAdjustmentCommand(Guid Id): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.ProductAdjustment;
}

internal sealed class DeleteProductAdjustmentCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteProductAdjustmentCommand>

{
    public async Task<Result> Handle(DeleteProductAdjustmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ProductAdjustments
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.ProductAdjustments.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}