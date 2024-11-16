namespace EasyPOS.Application.Features.SaleReturns.Commands;

public record DeleteSaleReturnDetailCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SaleReturn;
}

internal sealed class DeleteSaleReturnDetailCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<DeleteSaleReturnDetailCommand>
{
    public async Task<Result> Handle(DeleteSaleReturnDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.SaleReturnDetails.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.SaleReturnDetails.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
