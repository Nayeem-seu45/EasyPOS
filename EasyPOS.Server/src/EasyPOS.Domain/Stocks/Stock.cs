using EasyPOS.Domain.Products;

namespace EasyPOS.Domain.Stocks;

public class Stock: BaseAuditableEntity
{
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal AverageUnitCost { get; set; }
    public string? BatchNo { get; set; }
    public DateOnly? ExpiredDate { get; set; }

    public virtual Product Product { get; set; } = default!;
    public virtual Warehouse Warehouse { get; set; } = default!;
}
