namespace EasyPOS.Domain.Products;

public class CountStockCategory: BaseEntity
{
    public Guid CountStockId { get; set; }
    public Guid CategoryId { get; set; }

    public CountStock CountStock { get; set; } = default!;
}
