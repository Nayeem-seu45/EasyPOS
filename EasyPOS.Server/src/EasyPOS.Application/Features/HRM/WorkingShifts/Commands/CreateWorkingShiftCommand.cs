using EasyPOS.Domain.HRM;

namespace EasyPOS.Application.Features.HRM.WorkingShifts.Commands;

public record CreateWorkingShiftCommand(
    string? ShiftName, 
    string? Description, 
    bool IsActive
    ): ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.WorkingShift;
}
    
internal sealed class CreateWorkingShiftCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateWorkingShiftCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateWorkingShiftCommand request, CancellationToken cancellationToken)
    {
       var entity = request.Adapt<WorkingShift>();

       dbContext.WorkingShifts.Add(entity);

       await dbContext.SaveChangesAsync(cancellationToken);

       return  entity.Id;
    }
}