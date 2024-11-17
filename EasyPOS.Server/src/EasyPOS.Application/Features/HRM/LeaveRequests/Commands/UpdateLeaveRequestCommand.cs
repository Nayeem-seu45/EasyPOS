namespace EasyPOS.Application.Features.HRM.LeaveRequests.Commands;

public record UpdateLeaveRequestCommand(
    Guid Id,
    Guid EmployeeId,
    Guid LeaveTypeId,
    DateOnly StartDate,
    DateOnly EndDate,
    int TotalDays,
    Guid? StatusId,
    string? AttachmentUrl,
    string? Reason
    ) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.LeaveRequest;
}

internal sealed class UpdateLeaveRequestCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateLeaveRequestCommand>
{
    public async Task<Result> Handle(UpdateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LeaveRequests.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
