using EasyPOS.Domain.Stocks;

namespace EasyPOS.Domain.Products;

public class Warehouse : BaseAuditableEntity
{
    public string Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNo { get; set; }
    public string? Mobile { get; set; }
    public Guid? CountryId { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }

    public virtual List<Stock> Stocks { get; set; } = [];

}
