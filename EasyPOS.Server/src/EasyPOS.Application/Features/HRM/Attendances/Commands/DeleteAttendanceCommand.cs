namespace EasyPOS.Application.Features.HRM.Attendances.Commands;

public record DeleteAttendanceCommand(Guid Id): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Attendance;
}

internal sealed class DeleteAttendanceCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteAttendanceCommand>

{
    public async Task<Result> Handle(DeleteAttendanceCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Attendances
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.Attendances.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}