﻿using EasyPOS.Application.Common.Enums;

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
    string? Reason,
    bool IsSubmitted = false
    ) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.LeaveRequest;
}

internal sealed class UpdateLeaveRequestCommandHandler(
    IApplicationDbContext dbContext,
    ICommonQueryService commonQueryService) 
    : ICommandHandler<UpdateLeaveRequestCommand>
{
    public async Task<Result> Handle(UpdateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LeaveRequests.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

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

        return Result.Success();
    }
}
