using EasyPOS.Domain.HRM;

namespace EasyPOS.Application.Features.HRM.Employees.Commands;

public record CreateEmployeeCommand(
    string? Code, 
    string FirstName, 
    string? LastName, 
    string? Gender, 
    string? NID, 
    DateOnly? DOB, 
    Guid? WarehouseId, 
    Guid? DepartmentId, 
    Guid? DesignationId, 
    Guid? WorkingShiftId, 
    Guid? ReportTo, 
    string? Email, 
    string? PhoneNo, 
    string? MobileNo, 
    string? Country, 
    string? City, 
    string? Address
    ): ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Employee;
}
    
internal sealed class CreateEmployeeCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateEmployeeCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
       var entity = request.Adapt<Employee>();

       dbContext.Employees.Add(entity);

       await dbContext.SaveChangesAsync(cancellationToken);

       return  entity.Id;
    }
}
