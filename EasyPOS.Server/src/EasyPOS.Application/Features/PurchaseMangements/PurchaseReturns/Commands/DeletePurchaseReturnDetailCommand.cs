namespace EasyPOS.Application.Features.PurchaseReturns.Commands;

public record DeletePurchaseReturnDetailCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchaseReturn;
}

internal sealed class DeletePurchaseReturnDetailCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<DeletePurchaseReturnDetailCommand>
{
    public async Task<Result> Handle(DeletePurchaseReturnDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.PurchaseReturnDetails.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.PurchaseReturnDetails.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
