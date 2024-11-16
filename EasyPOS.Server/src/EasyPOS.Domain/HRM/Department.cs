namespace EasyPOS.Domain.HRM;

public class Department: BaseAuditableEntity
{
    public string Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public bool Status { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? DepartmentHeadId { get; set; }

}
