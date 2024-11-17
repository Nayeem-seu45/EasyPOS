namespace EasyPOS.Application.Features.HRM.Holidays.Queries;

public record HolidayModel
{
    public Guid Id { get; set; }
    public string? Title {get;set;}
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Description {get;set;} 
    public bool IsActive {get;set;} 
    public string? ActiveStatus {get;set;} 


    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

