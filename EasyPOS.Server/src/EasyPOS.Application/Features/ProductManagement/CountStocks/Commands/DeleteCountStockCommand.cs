namespace EasyPOS.Application.Features.ProductManagement.CountStocks.Commands;

public record DeleteCountStockCommand(Guid Id): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.CountStock;
}

internal sealed class DeleteCountStockCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteCountStockCommand>

{
    public async Task<Result> Handle(DeleteCountStockCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.CountStocks
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.CountStocks.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}