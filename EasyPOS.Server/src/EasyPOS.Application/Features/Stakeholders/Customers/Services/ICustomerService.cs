using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Domain.Stakeholders;

namespace EasyPOS.Application.Features.Stakeholders.Customers.Services;

internal interface ICustomerService
{
    void AdjustCustomerOnSale(
        SaleTransactionType transactionType,
        Customer customer,
        decimal dueAmount,
        decimal paidAmount = 0);

    void AdjustCustomerOnPayment(
        SaleTransactionType transactionType,
        Customer customer,
        decimal amount);
}


