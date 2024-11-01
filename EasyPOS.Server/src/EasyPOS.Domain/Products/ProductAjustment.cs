namespace EasyPOS.Domain.Products;

public class ProductAdjustment : BaseAuditableEntity
{
    public string ReferenceNo { get; set; }
    public decimal TotalQuantity { get; set; }
    public Guid WarehouseId { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? Note { get; set; }
    public DateTime AdjDate { get; set; }

    public virtual List<ProductAdjustmentDetail> ProductAdjustmentDetails { get; set; } = default!;
}
