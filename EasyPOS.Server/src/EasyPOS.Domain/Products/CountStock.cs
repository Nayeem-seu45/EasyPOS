namespace EasyPOS.Domain.Products;

public class CountStock: BaseAuditableEntity
{
    public string ReferenceNo { get; set; }
    public Guid WarehouseId { get; set; }
    public DateTime CountingDate { get; set; }
    public CountStockType Type { get; set; }

    public List<CountStockCategory> CountStockCategories { get; set; } = [];
    public List<CountStockBrand> CountStockBrands { get; set; } = [];
}
