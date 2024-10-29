namespace EasyPOS.Application.Features.Admin.AppPages.Commands;

public record DeleteAppPageCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.AppPage;
}

internal sealed class DeleteAppPageCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<DeleteAppPageCommand>
{
    public async Task<Result> Handle(DeleteAppPageCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.AppPages.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.AppPages.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
