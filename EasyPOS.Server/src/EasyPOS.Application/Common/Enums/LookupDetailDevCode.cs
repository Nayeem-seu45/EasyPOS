namespace EasyPOS.Application.Common.Enums;

public enum LookupDetailDevCode
{
}

public enum PaymentStatus
{
    Pending = 108101,
    Due = 108102,
    Partial = 108103,
    Paid = 108104
}

public enum SaleSatus
{
    Completed = 107101,
    Pending = 107102
}

public enum PaymentType
{
    Cach = 109101,
    Card = 109102,
    Cheque = 109103
}

public enum QuotationStatus
{
    Pending = 113001,
    Send = 113002
}

public enum PurchaseReturnStatus
{
    Completed = 115001,
    Pending = 115002
}

public enum SaleReturnStatus
{
    Received = 116001,
    Pending = 116002
}

public enum SalePaymentStatus
{
    Pending = 117001,
    Partial = 117002,
    Paid = 117003
}

public enum Gender
{
    Male = 118001,
    Female = 118002,
    Other = 118003
}

public enum LeaveStatus
{
    Drafted = 12001,
    Submitted = 12002,
    Forwarded = 12003,
    Approved = 12004,
    Rejected = 12005
}
