namespace EasyPOS.Application.Features.HRM.Departments.Commands;

public record DeleteDepartmentCommand(Guid Id): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Department;
}

internal sealed class DeleteDepartmentCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteDepartmentCommand>

{
    public async Task<Result> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Departments
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.Departments.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}