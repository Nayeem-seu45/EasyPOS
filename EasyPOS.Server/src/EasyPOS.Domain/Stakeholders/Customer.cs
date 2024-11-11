namespace EasyPOS.Domain.Stakeholders;

public class Customer : BaseAuditableEntity
{
    public string Name { get; set; }
    public string? Email { get; set; }
    public string PhoneNo { get; set; }
    public string? Mobile { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }

    public decimal TotalDueAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal DepositedBalance { get; set; }
    public decimal PreviousDue { get; set; }
    public decimal? CreditLimit { get; set; }
    public int? RewardPoints { get; set; }
    public bool IsActive { get; set; }

    private decimal CalculateOutstandingBalance()
    {
        return TotalDueAmount - TotalPaidAmount;
    }

}
