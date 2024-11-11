using EasyPOS.Application.Features.Stakeholders.Suppliers.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyPOS.Application.Features.Stakeholders.Suppliers.Services;

internal class SupplierFinancialService(IApplicationDbContext dbContext)
    : ISupplierFinancialService
{
    public async Task AdjustSupplierBalance(
        Guid supplierId, 
        decimal amount, 
        FinancialTransactionType transactionType, 
        CancellationToken cancellation = default)
    {
        var supplier = await dbContext.Suppliers.FindAsync(supplierId);
        if (supplier == null) return;

        switch (transactionType)
        {
            case FinancialTransactionType.Purchase:
                supplier.TotalDueAmount += amount;
                break;
            case FinancialTransactionType.PurchaseUpdate:
                supplier.TotalDueAmount += amount;
                break;
            case FinancialTransactionType.PurchaseDelete:
                supplier.TotalDueAmount -= amount;
                break;
            case FinancialTransactionType.Payment:
                supplier.TotalPaidAmount += amount; 
                supplier.TotalDueAmount -= amount;
                break;
            case FinancialTransactionType.PaymentUpdate:
                supplier.TotalPaidAmount += amount; 
                supplier.TotalDueAmount -= amount;
                break;
            case FinancialTransactionType.PaymentDelete:
                supplier.TotalPaidAmount -= amount;
                supplier.TotalDueAmount += amount;
                break;
            default: throw new InvalidOperationException("Unknown transaction type.");
        }

        // Recalculate OutstandingBalance
        supplier.OutstandingBalance = supplier.CalculateOutstandingBalance();
    }
}
