using EasyPOS.Domain.HRM;

namespace EasyPOS.Application.Features.HRM.LeaveTypes.Commands;

public record CreateLeaveTypeCommand(
    string Name, 
    string? Code, 
    int TotalLeaveDays, 
    int? MaxConsecutiveDays, 
    bool IsSandwichAllowed
    ): ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.LeaveType;
}
    
internal sealed class CreateLeaveTypeCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateLeaveTypeCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateLeaveTypeCommand request, CancellationToken cancellationToken)
    {
       var entity = request.Adapt<LeaveType>();

       dbContext.LeaveTypes.Add(entity);

       await dbContext.SaveChangesAsync(cancellationToken);

       return  entity.Id;
    }
}
