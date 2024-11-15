using EasyPOS.Application.Features.PurchaseMangements.Shared;

namespace EasyPOS.Application.Features.Stakeholders.Suppliers.Services;

internal class SupplierService(IApplicationDbContext dbContext)
    : ISupplierService
{
    public async Task AdjustSupplierBalance(
        Guid supplierId, 
        decimal amount, 
        PurchaseTransactionType transactionType, 
        CancellationToken cancellation = default)
    {
        var supplier = await dbContext.Suppliers.FindAsync(supplierId);
        if (supplier == null) return;

        switch (transactionType)
        {
            case PurchaseTransactionType.Purchase:
                supplier.TotalDueAmount += amount;
                break;
            case PurchaseTransactionType.PurchaseUpdate:
                supplier.TotalDueAmount += amount;
                break;
            case PurchaseTransactionType.PurchaseDelete:
                supplier.TotalDueAmount -= amount;
                break;
            case PurchaseTransactionType.Payment:
                supplier.TotalPaidAmount += amount; 
                supplier.TotalDueAmount -= amount;
                break;
            case PurchaseTransactionType.PaymentUpdate:
                supplier.TotalPaidAmount += amount; 
                supplier.TotalDueAmount -= amount;
                break;
            case PurchaseTransactionType.PaymentDelete:
                supplier.TotalPaidAmount -= amount;
                supplier.TotalDueAmount += amount;
                break;
            default: throw new InvalidOperationException("Unknown transaction type.");
        }

        // Recalculate OutstandingBalance
        supplier.OutstandingBalance = supplier.CalculateOutstandingBalance();
    }
}
