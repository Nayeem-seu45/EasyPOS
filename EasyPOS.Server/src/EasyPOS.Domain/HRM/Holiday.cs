namespace EasyPOS.Domain.HRM;

public class Holiday: BaseAuditableEntity
{
    public string Title { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
