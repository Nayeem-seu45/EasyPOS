namespace EasyPOS.Domain.Accounting;

public class MoneyTransfer : BaseAuditableEntity
{
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransferDate { get; set; }
    public string? ReferenceNo { get; set; }
}
