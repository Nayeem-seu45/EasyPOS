namespace EasyPOS.Application.Features.HRM.LeaveTypes.Models;

public record LeaveTypeModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int TotalLeaveDays { get; set; }
    public int? MaxConsecutiveDays { get; set; }
    public bool IsSandwichAllowed { get; set; }
    public string SandwichAllowed { get; set; }



    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

