using EasyPOS.Domain.HRM;

namespace EasyPOS.Application.Features.HRM.Attendances.Commands;

public record CreateAttendanceCommand(
    DateOnly AttendanceDate,
    Guid EmployeeId,
    TimeOnly CheckIn,
    TimeOnly? CheckOut,
    Guid StatusId) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Attendance;
}
    
internal sealed class CreateAttendanceCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateAttendanceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
    {
       var entity = request.Adapt<Attendance>();

       dbContext.Attendances.Add(entity);

       await dbContext.SaveChangesAsync(cancellationToken);

       return  entity.Id;
    }
}
