namespace EasyPOS.Application.Features.Sales.Models;

public record AddSalePaymentModel
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public decimal ReceivedAmount { get; set; }
    public decimal PayingAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public Guid? PaymentType { get; set; }
    public string? Note { get; set; }
}

