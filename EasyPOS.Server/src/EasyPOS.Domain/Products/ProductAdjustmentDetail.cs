namespace EasyPOS.Domain.Products;

public class ProductAdjustmentDetail: BaseEntity
{
    public Guid ProductAdjustmentId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Quantity { get; set; }
    public decimal Stock { get; set; }
    public ProductAdjAction ActionType { get; set; }

    public virtual ProductAdjustment ProductAdjustment { get; set; } = default!;
}
