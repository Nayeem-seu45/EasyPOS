using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Application.Features.Sales.Models;

public record SaleModel
{
    public Guid Id { get; set; }
    public DateOnly SaleDate { get; set; }
    public string? ReferenceNo { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid BillerId { get; set; }
    public string? AttachmentUrl { get; set; }
    public Guid? SaleStatusId { get; set; }
    public Guid? PaymentStatusId { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? DiscountRate { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal? ShippingCost { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public string? SaleNote { get; set; }
    public string? StaffNote { get; set; }

    public string WarehouseName { get; set; }
    public string CustomerName { get; set; }
    public string SaleStatus { get; set; }
    public string PaymentStatus { get; set; }

    public List<SaleDetailModel> SaleDetails { get; set; } = [];


    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

