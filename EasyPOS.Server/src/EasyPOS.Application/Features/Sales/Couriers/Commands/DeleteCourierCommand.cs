namespace EasyPOS.Application.Features.Sales.Couriers.Commands;

public record DeleteCourierCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Courier;
}

internal sealed class DeleteCourierCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<DeleteCourierCommand>

{
    public async Task<Result> Handle(DeleteCourierCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Couriers
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.Couriers.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}