namespace EasyPOS.Application.Features.ProductTransfers.Commands;

public record DeleteProductTransferDetailCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.ProductTransfer;
}

internal sealed class DeleteProductTransferDetailCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<DeleteProductTransferDetailCommand>
{
    public async Task<Result> Handle(DeleteProductTransferDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ProductTransferDetails.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.ProductTransferDetails.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
