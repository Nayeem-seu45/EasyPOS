namespace EasyPOS.Domain.HRM;

public class LeaveType: BaseAuditableEntity
{
    public string Name { get; set; }
    public string? Code { get; set; }
    public int TotalLeaveDays { get; set; }
    public int MaxConsecutiveAllowed { get; set; }
    public bool IsSandwichAllowed { get; set; }
}
