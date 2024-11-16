namespace EasyPOS.Application.Features.HRM.Holidays.Queries;

public record HolidayModel
{
    public Guid Id { get; set; }
    public string? Title {get;set;} 
    public string? Description {get;set;} 
    public bool IsActive {get;set;} 


    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

