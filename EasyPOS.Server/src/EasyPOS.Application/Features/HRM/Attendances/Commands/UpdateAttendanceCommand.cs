namespace EasyPOS.Application.Features.HRM.Attendances.Commands;

public record UpdateAttendanceCommand(
    Guid Id,
    Guid EmployeeId, 
    Guid AttendanceStatusId
    ): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Attendance;
}

internal sealed class UpdateAttendanceCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateAttendanceCommand>
{
    public async Task<Result> Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Attendances.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}