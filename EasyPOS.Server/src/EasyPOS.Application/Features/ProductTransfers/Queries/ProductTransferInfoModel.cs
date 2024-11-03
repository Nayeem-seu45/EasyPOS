using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Application.Features.ProductTransfers.Queries;

public class ProductTransferInfoModel
{
    public Guid Id { get; set; }
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

    public string TransferStatus { get; set; }
    public decimal TotalQuantity { get; set; } = 0;
    public decimal TotalDiscount { get; set; } = 0;
    public decimal TotalTaxAmount { get; set; } = 0;
    public string TotalItems { get; set; } = "0";

    public List<ProductTransferDetailModel> ProductTransferDetails { get; set; } = [];

}
