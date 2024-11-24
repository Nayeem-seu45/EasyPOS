namespace EasyPOS.Domain.HRM;

public class LeaveType: BaseAuditableEntity
{
    public string Name { get; set; }
    public string? Code { get; set; }
    public int TotalLeaveDays { get; set; }
    public int? MaxConsecutiveDays { get; set; }
    public bool IsSandwichAllowed { get; set; }
}
