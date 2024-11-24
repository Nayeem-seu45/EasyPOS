using EasyPOS.Domain.HRM;
using static EasyPOS.Application.Common.Security.Permissions;

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
    string? Address,
    List<Guid> LeaveTypes
    ) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Employee;
}
    
internal sealed class CreateEmployeeCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateEmployeeCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        // Generate a new ID for the employee if not already set.
        var employeeId = Guid.NewGuid();

        // Adapt the request to an Employee entity and set the generated Id.
        var entity = request.Adapt<Employee>();
        entity.Id = employeeId;

        // Add the Employee entity to the database.
        dbContext.Employees.Add(entity);

        // Add leave types to the Employee if provided.
        if (request.LeaveTypes.Any())
        {
            var leaveTypes = MapLeaveTypesToEmployee(request.LeaveTypes, employeeId);
            dbContext.EmployeeLeaveTypes.AddRange(leaveTypes);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    private static IEnumerable<EmployeeLeaveType> MapLeaveTypesToEmployee(List<Guid> leaveTypes, Guid employeeId)
    {
        return leaveTypes.Select(leaveTypeId => new EmployeeLeaveType
        {
            EmployeeId = employeeId,
            LeaveTypeId = leaveTypeId
        });
    }
}
