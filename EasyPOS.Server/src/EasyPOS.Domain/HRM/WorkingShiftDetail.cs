namespace EasyPOS.Domain.HRM;

public class WorkingShiftDetail: BaseEntity
{
    public Guid WorkingShiftId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DaysOfWeek DayOfWeek { get; set; }
    public bool IsWeekend { get; set; }

    public WorkingShift WorkingShift { get; set; } = default!;

}
