﻿using EasyPOS.Application.Features.Settings.CompanyInfos.Queries;
using EasyPOS.Application.Features.Suppliers.Models;
using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Application.Features.PurchaseReturns.Models;

public class PurchaseReturnInfoModel
{
    public Guid Id { get; set; }
    public DateOnly PurchaseReturnDate { get; set; }
    public string ReferenceNo { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid PurchaseReturnStatusId { get; set; }
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
    public string PurchaseReturnStatus { get; set; }
    public string PaymentStatusId { get; set; }
    public decimal TotalQuantity { get; set; } = 0;
    public decimal TotalDiscount { get; set; } = 0;
    public decimal TotalTaxAmount { get; set; } = 0;
    public string TotalItems { get; set; } = "0";

    public CompanyInfoModel CompanyInfo { get; set; } = default!;
    public SupplierModel Supplier { get; set; } = default!;
    public List<PurchaseReturnDetailModel> PurchaseReturnDetails { get; set; } = [];

}
