﻿namespace EasyPOS.Domain.Stakeholders;

public  class Supplier : BaseAuditableEntity
{
    public string Name { get; set; }
    public string? Email { get; set; }
    public string PhoneNo { get; set; }
    public string? Mobile { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public decimal? OpeningBalance { get; set; }
    public bool IsActive { get; set; }
}
