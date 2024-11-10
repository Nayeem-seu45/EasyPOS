namespace EasyPOS.Domain.Stakeholders;

public class Supplier : BaseAuditableEntity
{
    public string Name { get; set; }
    public string? Email { get; set; }
    public string PhoneNo { get; set; }
    public string? Mobile { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalDueAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal PreviousBalance { get; set; }
    public bool IsActive { get; set; }

    public decimal CalculateOutstandingBalance()
    {
        return OpeningBalance + TotalDueAmount - TotalPaidAmount;
    }

}
