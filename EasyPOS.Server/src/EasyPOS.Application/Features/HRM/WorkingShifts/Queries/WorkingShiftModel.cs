namespace EasyPOS.Application.Features.HRM.WorkingShifts.Queries;

public record WorkingShiftModel
{
    public Guid Id { get; set; }
    public string? ShiftName {get;set;} 
    public string? Description {get;set;} 
    public bool IsActive {get;set;} 


    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

