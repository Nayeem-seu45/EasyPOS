namespace EasyPOS.Application.Features.Suppliers.Models;

public record SupplierModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Email { get; set; }
    public string PhoneNo { get; set; }
    public string? Mobile { get; set; }
    public string Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalDueAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal PreviousBalance { get; set; }
    public bool IsActive { get; set; }
    public string Active { get; set; }
    public Dictionary<string, object> OptionsDataSources { get; set; } = [];

}
