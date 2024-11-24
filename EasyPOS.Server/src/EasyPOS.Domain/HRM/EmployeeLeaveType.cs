namespace EasyPOS.Domain.HRM;

public class EmployeeLeaveType: BaseAuditableEntity
{
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
}
