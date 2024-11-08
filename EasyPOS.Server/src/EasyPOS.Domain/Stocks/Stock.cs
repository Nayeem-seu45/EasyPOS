namespace EasyPOS.Domain.Stocks;

public class Stock: BaseAuditableEntity
{
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string? BatchNo { get; set; }
    public Guid PurchaseUnit { get; set; }
    public decimal TotalCost { get; set; }
    public decimal? AverageUnitCost { get; set; }
}
