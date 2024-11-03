using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Domain.ProductTransfers;

public class ProductTransfer : BaseAuditableEntity
{
    public DateOnly TransferDate { get; set; }
    public string ReferenceNo { get; set; }
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public Guid TransferStatusId { get; set; }
    public string? AttachmentUrl { get; set; }
    public decimal SubTotal { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal? DiscountRate { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? ShippingCost { get; set; }
    public decimal GrandTotal { get; set; }
    public string? Note { get; set; }

    public virtual List<ProductTransferDetail> ProductTransferDetails { get; set; } = [];

}
