﻿namespace EasyPOS.Domain.HRM;

public class Employee: BaseAuditableEntity
{
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
    public string? PhotoUrl { get; set; }
    public string? UserId { get; set; }

}
