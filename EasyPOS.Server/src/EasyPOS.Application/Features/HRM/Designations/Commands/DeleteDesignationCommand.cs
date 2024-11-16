namespace EasyPOS.Application.Features.HRM.Designations.Commands;

public record DeleteDesignationCommand(Guid Id): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Designation;
}

internal sealed class DeleteDesignationCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteDesignationCommand>

{
    public async Task<Result> Handle(DeleteDesignationCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Designations
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.Designations.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}