using EasyPOS.Domain.Common.Enums;
using EasyPOS.Domain.Enums;

namespace EasyPOS.Application.Features.Quotations.Queries;

public record QuotationModel
{
    public Guid Id { get; set; }
    public DateOnly QuotationDate { get; set; }
    public string? ReferenceNo { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid BillerId { get; set; }
    public string? AttachmentUrl { get; set; }
    public Guid? QuotationStatusId { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? DiscountRate { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal? ShippingCost { get; set; }
    public decimal GrandTotal { get; set; }
    public string? QuotationNote { get; set; }

    public string WarehouseName { get; set; }
    public string CustomerName { get; set; }
    public string QuotationStatus { get; set; }

    public List<QuotationDetailModel> QuotationDetails { get; set; } = [];


    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}


public record UpsertQuotationModel
{
    public Guid Id { get; set; }
    public DateOnly QuotationDate { get; set; }
    public string? ReferenceNo { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid BillerId { get; set; }
    public string? AttachmentUrl { get; set; }
    public Guid? QuotationStatusId { get; set; }
    public decimal SubTotal { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public DiscountType? DiscountType { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? DiscountRate { get; set; }
    public decimal? ShippingCost { get; set; }
    public decimal GrandTotal { get; set; }
    public string? QuotationNote { get; set; }

    public List<QuotationDetailModel> QuotationDetails { get; set; } = [];
    public Dictionary<string, object> OptionsDataSources { get; set; } = [];

}

public class QuotationDetailModel
{
    public Guid Id { get; set; }
    public Guid QuotationId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductUnitCost { get; set; }
    public decimal ProductUnitPrice { get; set; }
    public Guid ProductUnitId { get; set; }
    public decimal ProductUnit { get; set; }
    public decimal ProductUnitDiscount { get; set; }
    public int Quantity { get; set; }
    public string BatchNo { get; set; } = string.Empty;
    public DateOnly? ExpiredDate { get; set; }
    public decimal NetUnitPrice { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal? DiscountRate { get; set; }
    public decimal DiscountAmount { get; set; }
    public TaxMethod TaxMethod { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Remarks { get; set; }

}

