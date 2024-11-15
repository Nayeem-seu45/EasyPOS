using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.PurchaseMangements.Services;

internal interface IPurchaseReturnService
{
    Task AdjustPurchaseReturnAsync(
        PurchaseReturn purchaseReturn,
        decimal payingAmount,
        PurchaseReturnTransactionType transactionType,
        CancellationToken cancellationToken = default);

    Task UpdatePurchaseReturnPaymentStatusId(
        PurchaseReturn purchaseReturn,
        CancellationToken cancellationToken = default);
}
