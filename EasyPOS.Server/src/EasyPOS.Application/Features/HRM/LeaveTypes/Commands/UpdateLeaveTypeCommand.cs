namespace EasyPOS.Application.Features.HRM.LeaveTypes.Commands;

public record UpdateLeaveTypeCommand(
    Guid Id,
    string Name, 
    string? Code, 
    int TotalLeaveDays, 
    int MaxConsecutiveAllowed, 
    bool IsSandwichAllowed
    ): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.LeaveType;
}

internal sealed class UpdateLeaveTypeCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateLeaveTypeCommand>
{
    public async Task<Result> Handle(UpdateLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LeaveTypes.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}