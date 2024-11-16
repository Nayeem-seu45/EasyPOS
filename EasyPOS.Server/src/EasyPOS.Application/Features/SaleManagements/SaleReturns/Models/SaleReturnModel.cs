using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Application.Features.SaleReturns.Models;

public record SaleReturnModel
{
    public Guid Id { get; set; }
    public DateOnly ReturnDate { get; set; }
    public string? SoldReferenceNo { get; set; }
    public string? ReferenceNo { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid BillerId { get; set; }
    public string? AttachmentUrl { get; set; }
    public Guid? ReturnStatusId { get; set; }
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
    public string? ReturnNote { get; set; }
    public string? StaffNote { get; set; }

    public string WarehouseName { get; set; }
    public string CustomerName { get; set; }
    public string ReturnStatus { get; set; }
    public string PaymentStatus { get; set; }
    public string PaymentStatusTag { get; set; }

    public List<SaleReturnDetailModel> SaleReturnDetails { get; set; } = [];


    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

