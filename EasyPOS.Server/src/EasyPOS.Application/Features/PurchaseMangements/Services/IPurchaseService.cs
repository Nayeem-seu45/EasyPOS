using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.PurchaseMangements.Services;

internal interface IPurchaseService
{
    Task AdjustPurchaseAsync(
        Purchase purchase, 
        decimal payingAmount, 
        PurchaseTransactionType transactionType,
        CancellationToken cancellationToken = default);

    Task UpdatePurchasePaymentStatusId(
        Purchase purchase,
        CancellationToken cancellationToken = default);

    Task<Guid?> GetPurchasePaymentId(Purchase purchase, CancellationToken cancellationToken = default);
}
