namespace EasyPOS.Domain.Trades;

public class Courier: BaseAuditableEntity
{
    public string Name { get; set; }
    public string PhoneNo { get; set; }
    public string MobileNo { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
}
