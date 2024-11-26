using EasyPOS.Domain.HRM;

namespace EasyPOS.Application.Features.HRM.Employees.Commands;

public record UpdateEmployeeCommand(
    Guid Id,
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
    string? UserId,
    List<Guid> LeaveTypes
    ) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Employee;
}

internal sealed class UpdateEmployeeCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateEmployeeCommand>
{
    public async Task<Result> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Employees.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        UpdateEmployeeLeaveTypes(dbContext, request);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static void UpdateEmployeeLeaveTypes(IApplicationDbContext dbContext, UpdateEmployeeCommand request)
    {
        if (request.LeaveTypes != null)
        {
            // Remove existing leave types associated with the employee
            var existingLeaveTypes = dbContext.EmployeeLeaveTypes
                .Where(e => e.EmployeeId == request.Id);

            dbContext.EmployeeLeaveTypes.RemoveRange(existingLeaveTypes);

            // Add the updated leave types
            if (request.LeaveTypes.Any())
            {
                var newLeaveTypes = request.LeaveTypes.Select(leaveTypeId => new EmployeeLeaveType
                {
                    EmployeeId = request.Id,
                    LeaveTypeId = leaveTypeId
                });

                dbContext.EmployeeLeaveTypes.AddRange(newLeaveTypes);
            }
        }
    }
}
