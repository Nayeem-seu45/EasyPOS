namespace EasyPOS.Domain.Purchases;

public class PurchaseReturnPayment : BaseAuditableEntity
{
    public Guid PurchaseReturnId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal ReceivedAmount { get; set; }
    public decimal PayingAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public Guid PaymentType { get; set; }
    public string? Note { get; set; }

    public virtual PurchaseReturn PurchaseReturn { get; set; } = default!;
}
