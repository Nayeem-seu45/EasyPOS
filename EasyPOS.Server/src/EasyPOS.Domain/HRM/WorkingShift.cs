namespace EasyPOS.Domain.HRM;

public class WorkingShift: BaseAuditableEntity
{
    public string ShiftName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    public List<WorkingShiftDetail> WorkingShiftDetails { get; set; } = [];

}
