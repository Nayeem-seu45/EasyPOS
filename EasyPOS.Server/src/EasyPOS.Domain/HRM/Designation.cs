namespace EasyPOS.Domain.HRM;

public class Designation: BaseAuditableEntity
{
    public string Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public bool Status { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid? ParentId { get; set; }

}
