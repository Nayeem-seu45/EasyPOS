namespace EasyPOS.Application.Features.HRM.WorkingShifts.Models;

public record WorkingShiftModel
{
    public Guid Id { get; set; }
    public string? ShiftName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? ActiveStatus { get; set; }

    public List<WorkingShiftDetailModel> WorkingShiftDetails { get; set; } = [];

    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

