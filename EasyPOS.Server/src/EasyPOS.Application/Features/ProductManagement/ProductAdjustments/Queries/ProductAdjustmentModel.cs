using EasyPOS.Domain.Products;

namespace EasyPOS.Application.Features.ProductManagement.ProductAdjustments.Queries;

public record ProductAdjustmentModel
{
    public Guid Id { get; set; }
    public string ReferenceNo { get; set; }
    public Guid WarehouseId {get;set;} 
    public string? AttachmentUrl {get;set;} 
    public string? Note {get;set;} 
    public DateTime AdjDate {get;set;}
    public decimal TotalQuantity { get; set; }
    public string Warehouse { get; set; }

    public List<ProductAdjustmentDetailModel> ProductAdjustmentDetails { get; set; } = [];
    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

public class ProductAdjustmentDetailModel
{
    public Guid Id { get; set; }
    public Guid ProductAdjustmentId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Quantity { get; set; }
    public decimal Stock { get; set; }
    public ProductAdjAction ActionType { get; set; }
}


