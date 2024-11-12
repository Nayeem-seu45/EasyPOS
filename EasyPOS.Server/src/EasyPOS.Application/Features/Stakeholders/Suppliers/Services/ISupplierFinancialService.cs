using EasyPOS.Application.Features.Stakeholders.Suppliers.Models;

namespace EasyPOS.Application.Features.Stakeholders.Suppliers.Services;

internal interface ISupplierFinancialService
{
    //Task IncreaseSupplierFinancials(Guid supplierId, decimal amount);
    //Task DecreaseSupplierFinancials(Guid supplierId, decimal amount);
    //Task AddDueAmountSupplierBalance(Guid supplierId, decimal amount);
    //Task UpdateSupplierFinancialsOnPurchaseUpdate(Guid supplierId, decimal dueAmountDifference, decimal paidAmountDifference);
    Task AdjustSupplierBalance(Guid supplierId, decimal amount, PurchaseTransactionType transactionType, CancellationToken cancellationToken = default!);

}
