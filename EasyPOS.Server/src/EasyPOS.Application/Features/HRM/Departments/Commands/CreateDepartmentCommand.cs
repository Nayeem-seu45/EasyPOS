using EasyPOS.Domain.HRM;

namespace EasyPOS.Application.Features.HRM.Departments.Commands;

public record CreateDepartmentCommand(
    string Name, 
    string? Code, 
    string? Description, 
    bool Status, 
    Guid? ParentId, 
    Guid? DepartmentHeadId
    ): ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Department;
}
    
internal sealed class CreateDepartmentCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateDepartmentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
       var entity = request.Adapt<Department>();

       dbContext.Departments.Add(entity);

       await dbContext.SaveChangesAsync(cancellationToken);

       return  entity.Id;
    }
}