namespace EasyPOS.Domain.Stocks;

public class StockTransaction: BaseAuditableEntity
{
    public Guid Id { get; set; }
    public Guid StockId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public bool IsAddition { get; set; }
}
