namespace EasyPOS.Application.Features.Sales.Couriers.Queries;

public record CourierModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhoneNo { get; set; }
    public string? MobileNo { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }


    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

