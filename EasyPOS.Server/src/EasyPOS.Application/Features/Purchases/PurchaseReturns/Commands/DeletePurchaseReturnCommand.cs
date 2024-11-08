namespace EasyPOS.Application.Features.PurchaseReturns.Commands;

public record DeletePurchaseReturnCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchaseReturn;
}

internal sealed class DeletePurchaseReturnCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<DeletePurchaseReturnCommand>
{
    public async Task<Result> Handle(DeletePurchaseReturnCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.PurchaseReturns.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.PurchaseReturns.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
