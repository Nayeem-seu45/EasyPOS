﻿namespace EasyPOS.Domain.Products;

public class Product : BaseAuditableEntity
{
    public string Name { get; set; }
    public Guid CategoryId { get; set; }
    public Guid ProductTypeId { get; set; }
    public Guid? BrandId { get; set; }
    public string? Code { get; set; }
    public string? SKU { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal Price { get; set; }
    public decimal? WholesalePrice { get; set; }
    public Guid? Unit { get; set; }
    public Guid? SaleUnit { get; set; }
    public Guid? PurchaseUnit { get; set; }
    public int? AlertQuantity { get; set; }
    public Guid? BarCodeType { get; set; }
    public Guid? QRCodeType { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }


    public virtual Category Category { get; set; } = default!;
}