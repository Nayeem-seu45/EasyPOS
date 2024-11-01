using EasyPOS.Domain.Products;

namespace EasyPOS.Application.Features.ProductManagement.CountStocks.Queries;

public record CountStockModel
{
    public Guid Id { get; set; }
    public string ReferenceNo { get; set; }
    public Guid WarehouseId {get;set;} 
    public string Warehouse {get;set;} 
    public DateTime CountingDate {get;set;}
    public CountStockType Type { get; set; }
    public string TypeName { get; set; }

    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

