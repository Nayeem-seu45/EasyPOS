namespace EasyPOS.Application.Features.HRM.Attendances.Queries;

public record AttendanceModel
{
    public Guid Id { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public Guid EmployeeId { get; set; }
    public TimeOnly? CheckIn { get; set; }
    public TimeOnly? CheckOut { get; set; }
    public Guid StatusId { get; set; }

    public string? EmployeeName { get; set; }

    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

