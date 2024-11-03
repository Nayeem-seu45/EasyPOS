namespace EasyPOS.Application.Features.ProductTransfers.Commands;

public record DeleteProductTransferCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.ProductTransfer;
}

internal sealed class DeleteProductTransferCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<DeleteProductTransferCommand>
{
    public async Task<Result> Handle(DeleteProductTransferCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ProductTransfers.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.ProductTransfers.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
