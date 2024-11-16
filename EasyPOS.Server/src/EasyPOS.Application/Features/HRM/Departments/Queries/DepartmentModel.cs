namespace EasyPOS.Application.Features.HRM.Departments.Queries;

public record DepartmentModel
{
    public Guid Id { get; set; }
    public string Name {get;set;} = string.Empty; 
    public string? Code {get;set;} 
    public string? Description {get;set;} 
    public bool Status {get;set;} 
    public Guid? ParentId {get;set;} 
    public Guid? DepartmentHeadId {get;set;} 


    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

