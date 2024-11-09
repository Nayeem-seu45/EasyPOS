namespace EasyPOS.Application.Features.SaleReturns.Commands;

public record DeleteSaleReturnCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SaleReturn;
}

internal sealed class DeleteSaleReturnCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<DeleteSaleReturnCommand>

{
    public async Task<Result> Handle(DeleteSaleReturnCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.SaleReturns
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.SaleReturns.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}
