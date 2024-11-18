namespace EasyPOS.Application.Features.HRM.WorkingShifts.Models;

public record WorkingShiftDetailModel
{
    public Guid Id { get; set; }
    public Guid WorkingShiftId { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string DayNameOfWeek { get; set; }
    public bool IsWeekend { get; set; }
}
