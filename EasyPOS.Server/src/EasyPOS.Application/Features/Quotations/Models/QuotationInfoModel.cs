using EasyPOS.Application.Features.Customers.Models;
using EasyPOS.Application.Features.Settings.CompanyInfos.Queries;
using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Application.Features.Quotations.Models;

public class QuotationInfoModel
{
    public Guid Id { get; set; }
    public DateOnly QuotationDate { get; set; }
    public string? ReferenceNo { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid BillerId { get; set; }
    public string? AttachmentUrl { get; set; }
    public Guid? QuotationStatusId { get; set; }
    public Guid? PaymentStatusId { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? DiscountRate { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal? ShippingCost { get; set; }
    public decimal SubTotal { get; set; }
    public decimal GrandTotal { get; set; }
    public string? QuotationNote { get; set; }
    public string? StaffNote { get; set; }

    public string WarehouseName { get; set; }
    public string CustomerName { get; set; }
    public string QuotationStatus { get; set; }
    public string PaymentStatus { get; set; }
    public decimal TotalQuantity { get; set; } = 0;
    public decimal TotalDiscount { get; set; } = 0;
    public decimal TotalTaxAmount { get; set; } = 0;
    public string TotalItems { get; set; } = "0";

    public CompanyInfoModel CompanyInfo { get; set; } = default!;
    public CustomerModel Customer { get; set; } = default!;
    public List<QuotationDetailModel> QuotationDetails { get; set; } = [];

}
