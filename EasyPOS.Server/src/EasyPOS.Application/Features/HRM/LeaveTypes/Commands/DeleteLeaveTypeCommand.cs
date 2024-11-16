namespace EasyPOS.Application.Features.HRM.LeaveTypes.Commands;

public record DeleteLeaveTypeCommand(Guid Id): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.LeaveType;
}

internal sealed class DeleteLeaveTypeCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteLeaveTypeCommand>

{
    public async Task<Result> Handle(DeleteLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.LeaveTypes
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.LeaveTypes.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}