namespace EasyPOS.Application.Features.HRM.LeaveTypes.Queries;

public record LeaveTypeSelectListModel
{
    public Guid Id { get; set; }
    public string Name {get;set;} = string.Empty; 
    public string? Code {get;set;} 
    public int TotalLeaveDays {get;set;} 
    public int MaxConsecutiveAllowed {get;set;} 
    public bool IsSandwichAllowed {get;set;} 
}

