using EasyPOS.Application.Features.Purchases.PurchasePayments.Queries;
using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Application.Features.Purchases.Models;

public record PurchaseModel
{
    public Guid Id { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public string ReferenceNo { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid PurchaseStatusId { get; set; }
    public string? AttachmentUrl { get; set; }
    public decimal SubTotal { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal? DiscountRate { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? ShippingCost { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public string? Note { get; set; }

    public string SupplierName { get; set; }
    public string PurchaseStatus { get; set; }
    public string PaymentStatus { get; set; }
    public string PaymentStatusTag { get; set; }

    public List<PurchaseDetailModel> PurchaseDetails { get; set; } = [];
    public List<PurchasePaymentModel> PaymentDetails { get; set; } = [];

    public Dictionary<string, object> OptionsDataSources { get; set; } = [];

}
