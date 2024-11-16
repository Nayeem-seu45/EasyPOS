namespace EasyPOS.Domain.HRM;

public class LeaveRequest: BaseAuditableEntity
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int TotalDays { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public Guid StatusId { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? Reason { get; set; }
}
