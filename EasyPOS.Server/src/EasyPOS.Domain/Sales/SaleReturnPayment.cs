namespace EasyPOS.Domain.Sales;

public class SaleReturnPayment : BaseAuditableEntity
{
    public Guid SaleReturnId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal ReceivedAmount { get; set; }
    public decimal PayingAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public Guid PaymentType { get; set; }
    public string? Note { get; set; }

    public virtual SaleReturn SaleReturn { get; set; } = default!;
}
