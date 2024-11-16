namespace EasyPOS.Application.Features.HRM.WorkingShifts.Commands;

public record UpdateWorkingShiftCommand(
    Guid Id,
    string? ShiftName, 
    string? Description, 
    bool IsActive
    ): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.WorkingShift;
}

internal sealed class UpdateWorkingShiftCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateWorkingShiftCommand>
{
    public async Task<Result> Handle(UpdateWorkingShiftCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.WorkingShifts.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}