namespace EasyPOS.Application.Features.HRM.LeaveRequests.Commands;

public record DeleteLeaveRequestCommand(Guid Id): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.LeaveRequest;
}

internal sealed class DeleteLeaveRequestCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteLeaveRequestCommand>

{
    public async Task<Result> Handle(DeleteLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LeaveRequests
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.LeaveRequests.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}
