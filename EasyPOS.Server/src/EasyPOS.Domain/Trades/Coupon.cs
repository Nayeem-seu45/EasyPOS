namespace EasyPOS.Domain.Trades;

public class Coupon : BaseAuditableEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int DiscountType { get; set; }
    public decimal Amount { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool AllowFreeShipping { get; set; }
    public decimal? MinimumSpend { get; set; }
    public decimal? MaximumSpend { get; set; }
    public bool OnlyIndivisual { get; set; }
    public decimal? PerCouponUsageLimit { get; set; }
    public decimal? PerUserUsageLimit { get; set; }
}
