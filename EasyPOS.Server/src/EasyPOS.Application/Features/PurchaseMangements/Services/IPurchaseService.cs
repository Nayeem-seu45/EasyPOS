using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.PurchaseMangements.Services;

internal interface IPurchaseService
{
    Task AdjustPurchaseAndPaymentStatusAsync(
        Purchase purchase, 
        decimal payingAmount, 
        PurchaseTransactionType transactionType,
        CancellationToken cancellationToken = default);

    Task<Guid?> GetPurchasePaymentId(Purchase purchase, CancellationToken cancellationToken = default);
}
