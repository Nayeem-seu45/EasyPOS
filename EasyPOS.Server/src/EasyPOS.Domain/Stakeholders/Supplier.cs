namespace EasyPOS.Domain.Stakeholders;

public  class Supplier : BaseAuditableEntity
{
    public string Name { get; set; }
    public string? Email { get; set; }
    public string PhoneNo { get; set; }
    public string? Mobile { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public decimal? OpeningBalance { get; set; } // Initial balance with the supplier
    public decimal? TotalDueAmount { get; set; } // Total amount due to the supplier
    public decimal? PaidAmount { get; set; } // Amount paid against the dues
    public decimal? TotalAdvanceAmount { get; set; } // Total advance paid to the supplier
    public decimal OutstandingBalance { get; set; }
    public decimal? PreviousBalance { get; set; } // Previous balance, if needed
    public decimal? CreditLimit { get; set; } // Optional credit limit for the supplier
    public bool IsActive { get; set; }
}
