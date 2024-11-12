namespace EasyPOS.Domain.Stakeholders;

public class Customer : BaseAuditableEntity
{
    public string? IdentityNo { get; set; }
    public string Name { get; set; }
    public string? Email { get; set; }
    public string PhoneNo { get; set; }
    public string? Mobile { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public decimal? CreditLimit { get; set; }

    // Financial fields for tracking purchases, payments, and returns
    public decimal TotalDueAmount { get; set; }         // Sum of due amounts from transactions
    public decimal TotalSaleReturnAmount { get; set; }   // Adjusts balance after sale returns
    public decimal TotalPaidAmount { get; set; }         // Sum of all payments made by the customer
    public int? RewardPoints { get; set; }
    public bool IsActive { get; set; }


}
