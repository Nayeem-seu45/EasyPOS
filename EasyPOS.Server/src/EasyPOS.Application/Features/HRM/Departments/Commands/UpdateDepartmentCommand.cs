namespace EasyPOS.Application.Features.HRM.Departments.Commands;

public record UpdateDepartmentCommand(
    Guid Id,
    string Name, 
    string? Code, 
    string? Description, 
    bool Status, 
    Guid? ParentId, 
    Guid? DepartmentHeadId
    ): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Department;
}

internal sealed class UpdateDepartmentCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateDepartmentCommand>
{
    public async Task<Result> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Departments.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}