using EasyPOS.Domain.HRM;

namespace EasyPOS.Application.Features.HRM.Designations.Commands;

public record CreateDesignationCommand(
    string Name, 
    string? Code, 
    string? Description, 
    bool Status, 
    Guid DepartmentId, 
    Guid? ParentId
    ): ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Designation;
}
    
internal sealed class CreateDesignationCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateDesignationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateDesignationCommand request, CancellationToken cancellationToken)
    {
       var entity = request.Adapt<Designation>();

       dbContext.Designations.Add(entity);

       await dbContext.SaveChangesAsync(cancellationToken);

       return  entity.Id;
    }
}