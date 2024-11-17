using EasyPOS.Domain.HRM;

namespace EasyPOS.Application.Features.HRM.LeaveRequests.Commands;

public record CreateLeaveRequestCommand(
    Guid EmployeeId,
    Guid LeaveTypeId,
    DateOnly StartDate,
    DateOnly EndDate,
    int TotalDays,
    Guid? StatusId,
    string? AttachmentUrl,
    string? Reason
    ) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.LeaveRequest;
}

internal sealed class CreateLeaveRequestCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<CreateLeaveRequestCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<LeaveRequest>();

        dbContext.LeaveRequests.Add(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
