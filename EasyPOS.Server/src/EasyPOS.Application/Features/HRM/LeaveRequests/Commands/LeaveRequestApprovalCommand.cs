using EasyPOS.Application.Common.Enums;

namespace EasyPOS.Application.Features.HRM.LeaveRequests.Commands;

public record LeaveRequestApprovalCommand(
    Guid Id,
    DateOnly StartDate,
    DateOnly EndDate,
    int TotalDays,
    string? Reason,
    LeaveStatus ApprovalAction
    ) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.LeaveRequest;
}

internal sealed class LeaveRequestApprovalCommandHandler(
    IApplicationDbContext dbContext,
    ICommonQueryService commonQueryService) 
    : ICommandHandler<LeaveRequestApprovalCommand>
{
    public async Task<Result> Handle(LeaveRequestApprovalCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LeaveRequests.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        // Get StatusId based on status type
        var leaveStatusId = await commonQueryService.GetLookupDetailIdAsync((int)request.ApprovalAction, cancellationToken);
        if (leaveStatusId.IsNullOrEmpty())
        {
            return Result.Failure<Guid>(
                Error.Failure(ErrorMessages.NotFound, "Leave Status not found")
            );
        }

        entity.StatusId = leaveStatusId.Value;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
