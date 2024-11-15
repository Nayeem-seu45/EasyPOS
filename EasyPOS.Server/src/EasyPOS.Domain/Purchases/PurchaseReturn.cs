using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Domain.Purchases;

public class PurchaseReturn : BaseAuditableEntity
{
    public DateOnly ReturnDate { get; set; }
    public string PurchaseReferenceNo { get; set; }
    public string ReferenceNo { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid PurchaseId { get; set; }
    public string? AttachmentUrl { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public Guid? ReturnStatusId { get; set; }
    public Guid? PaymentStatusId { get; set; }
    public string? Note { get; set; }

    public Purchase Purchase { get; set; } = default!;
    public virtual List<PurchaseReturnDetail> PurchaseReturnDetails { get; set; } = [];
    public virtual List<PurchaseReturnPayment> PurchaseReturnPayments { get; set; } = [];


}
