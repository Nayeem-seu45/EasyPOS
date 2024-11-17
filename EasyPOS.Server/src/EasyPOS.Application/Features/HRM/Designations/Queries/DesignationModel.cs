namespace EasyPOS.Application.Features.HRM.Designations.Queries;

public record DesignationModel
{
    public Guid Id { get; set; }
    public string Name {get;set;} = string.Empty; 
    public string? Code {get;set;} 
    public string? Description {get;set;} 
    public bool Status {get;set;} 
    public Guid DepartmentId {get;set;} 
    public string? Department {get;set;} 
    public Guid? ParentId {get;set;} 


    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

