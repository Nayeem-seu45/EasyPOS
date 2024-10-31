namespace EasyPOS.Domain.Trades;

public class GiftCard: BaseAuditableEntity
{
    public string CardNo { get; set; }
    public decimal Amount { get; set; }
    public decimal Expense { get; set; }
    public decimal Balance { get; set; }
    public DateTime ExpiredDate { get; set; }
    public Guid CustomerId { get; set; }
    public bool AllowMultipleTransac { get; set; }
    public Guid? Status { get; set; }
}
