using EasyPOS.Application.Common.Enums;

namespace EasyPOS.Application.Features.HRM.LeaveRequests.Queries;

public record LeaveRequestModel
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int TotalDays {get;set;} 
    public Guid? StatusId {get;set;} 
    public string? AttachmentUrl {get;set;} 
    public string? Reason {get;set;}

    public string? EmployeeName { get; set; }
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public LeaveStatus LeaveStatus { get; set; }
    public string? Status { get; set; }


    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

