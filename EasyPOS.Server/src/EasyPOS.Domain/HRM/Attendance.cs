namespace EasyPOS.Domain.HRM;

public class Attendance: BaseAuditableEntity
{
    public DateOnly AttendanceDate { get; set; }
    public Guid EmployeeId { get; set; }
    public TimeOnly? CheckIn { get; set; }
    public TimeOnly? CheckOut { get; set; }
    public Guid StatusId { get; set; }
}
