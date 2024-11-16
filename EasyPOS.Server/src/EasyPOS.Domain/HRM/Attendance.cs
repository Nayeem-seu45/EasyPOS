namespace EasyPOS.Domain.HRM;

public class Attendance: BaseAuditableEntity
{
    public DateOnly AttendanceDate { get; set; }
    public Guid EmployeeId { get; set; }
    public TimeSpan CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public Guid AttendanceStatusId { get; set; }
}
