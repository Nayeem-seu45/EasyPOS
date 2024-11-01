namespace EasyPOS.Domain.Products;

public class CountStockBrand: BaseEntity
{
    public Guid CountStockId { get; set; }
    public Guid BrandId { get; set; }

    public CountStock CountStock { get; set; } = default!;

}
