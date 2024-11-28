using EasyPOS.Application.Common.Enums;
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
    string? Reason,
    bool IsSubmitted = false
    ) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.LeaveRequest;
}

internal sealed class CreateLeaveRequestCommandHandler(
    IApplicationDbContext dbContext,
    ICommonQueryService commonQueryService)
    : ICommandHandler<CreateLeaveRequestCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<LeaveRequest>();

        dbContext.LeaveRequests.Add(entity);

        // Determine status based on IsSubmitted flag
        var statusType = request.IsSubmitted ? LeaveStatus.Submitted : LeaveStatus.Initiated;

        // Get StatusId based on status type
        var leaveStatusId = await commonQueryService.GetLookupDetailIdAsync((int)statusType, cancellationToken);
        if (leaveStatusId.IsNullOrEmpty())
        {
            return Result.Failure<Guid>(
                Error.Failure(ErrorMessages.NotFound, "Leave Status not found")
            );
        }

        entity.StatusId = leaveStatusId.Value;

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
