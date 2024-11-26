namespace EasyPOS.Application.Features.HRM.Employees.Queries;

public record EmployeeModel
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public string Gender { get; set; }
    public DateOnly? DOB { get; set; }
    public string? NID { get; set; }
    public Guid? ReportTo { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? DesignationId { get; set; }
    public Guid? WorkingShiftId { get; set; }
    public string? Email { get; set; }
    public string? PhoneNo { get; set; }
    public string? MobileNo { get; set; }
    public string Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? UserId { get; set; }
    public string? PhotoUrl { get; set; }


    public string? Warehouse { get; set; }
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public string? WorkingShift { get; set; }

    public List<Guid> LeaveTypes { get; set; } = [];

    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

