namespace EasyPOS.Application.Features.ProductManagement.CountStocks.Commands;

public record UpdateCountStockCommand(
    Guid Id,
    Guid WarehouseId, 
    DateTime CountingDate
    ): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.CountStock;
}

internal sealed class UpdateCountStockCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateCountStockCommand>
{
    public async Task<Result> Handle(UpdateCountStockCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.CountStocks.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}