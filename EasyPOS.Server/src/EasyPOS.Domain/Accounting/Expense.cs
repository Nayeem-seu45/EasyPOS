namespace EasyPOS.Domain.Accounting;

public class Expense: BaseAuditableEntity
{
    public DateTime ExpenseDate { get; set; }
    public string Title { get; set; }
    public string ReferenceNo { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public Guid AccountId { get; set; }
    public string? Description { get; set; }
    public string? AttachmentUrl { get; set; }
}
