using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Application.Features.PurchaseReturns.Models;

public record PurchaseReturnModel
{
    public Guid Id { get; set; }
    public Guid PurchaseId { get; set; }
    public DateOnly ReturnDate { get; set; }
    public string ReferenceNo { get; set; }
    public string PurchaseReferenceNo { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid ReturnStatusId { get; set; }
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

    public string SupplierName { get; set; }
    public string ReturnStatus { get; set; }

    public List<PurchaseReturnDetailModel> PurchaseReturnDetails { get; set; } = [];

    public Dictionary<string, object> OptionsDataSources { get; set; } = [];

}
