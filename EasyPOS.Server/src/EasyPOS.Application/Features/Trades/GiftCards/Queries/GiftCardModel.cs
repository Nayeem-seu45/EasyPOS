namespace EasyPOS.Application.Features.Trades.GiftCards.Queries;

public record GiftCardModel
{
    public Guid Id { get; set; }
    public string? CardNo {get;set;} 
    public decimal Amount {get;set;} 
    public decimal Expense {get;set;} 
    public decimal Balance {get;set;} 
    public DateTime ExpiredDate {get;set;} 
    public Guid? CustomerId {get;set;} 
    public bool AllowMultipleTransac {get;set;} 
    public Guid GiftCardType {get;set;} 
    public Guid Status {get;set;} 


    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

