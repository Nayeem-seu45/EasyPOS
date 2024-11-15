using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Domain.Stakeholders;

namespace EasyPOS.Application.Features.Stakeholders.Customers.Services;

internal sealed class CustomerService(IApplicationDbContext dbContext) 
    : ICustomerService
{
    public void AdjustCustomerOnSale(
        SaleTransactionType transactionType,
        Customer customer,
        decimal dueAmount,
        decimal paidAmount = 0)
    {
        switch (transactionType)
        {
            case SaleTransactionType.SaleCreate:
                customer.TotalDueAmount += dueAmount;
                customer.TotalPaidAmount += paidAmount;
                break;
            case SaleTransactionType.SaleUpdate:
                customer.TotalDueAmount += dueAmount;
                break;
            case SaleTransactionType.SaleDelete:
                customer.TotalDueAmount -= dueAmount;
                break;
            default:
                throw new InvalidOperationException("Unknown transaction type.");
        }
    }

    public void AdjustCustomerOnPayment(
        SaleTransactionType transactionType,
        Customer customer,
        decimal amount)
    {
        switch (transactionType)
        {
            case SaleTransactionType.PaymentCreate:
                customer.TotalPaidAmount += amount;
                customer.TotalDueAmount -= amount;
                break;
            case SaleTransactionType.PaymentUpdate:
                customer.TotalPaidAmount += amount;
                customer.TotalDueAmount -= amount;
                break;
            case SaleTransactionType.PaymentDelete:
                customer.TotalPaidAmount -= amount;
                customer.TotalDueAmount += amount;
                break;
            default:
                throw new InvalidOperationException("Unknown transaction type.");
        }
    }
}
