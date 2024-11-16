namespace EasyPOS.Application.Features.HRM.Designations.Commands;

public record UpdateDesignationCommand(
    Guid Id,
    string Name, 
    string? Code, 
    string? Description, 
    bool Status, 
    Guid DepartmentId, 
    Guid? ParentId
    ): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Designation;
}

internal sealed class UpdateDesignationCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateDesignationCommand>
{
    public async Task<Result> Handle(UpdateDesignationCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Designations.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}