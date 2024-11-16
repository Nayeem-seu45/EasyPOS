namespace EasyPOS.Application.Features.HRM.Attendances.Queries;

public record AttendanceModel
{
    public Guid Id { get; set; }
    public Guid EmployeeId {get;set;} 
    public Guid AttendanceStatusId {get;set;} 


    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

