using EasyPOS.Application.Common.Enums;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.PurchaseMangements.Services;

public class PurchaseReturnService(
    ICommonQueryService commonQueryService) : IPurchaseReturnService
{
    public async Task AdjustPurchaseReturnAsync(
        PurchaseReturn purchaseReturn, 
        decimal payingAmount,
        PurchaseReturnTransactionType transactionType, 
        CancellationToken cancellationToken = default)
    {

        switch (transactionType)
        {
            case PurchaseReturnTransactionType.PurchaseReturnCreate:
            case PurchaseReturnTransactionType.ReturnPaymentCreate:
                purchaseReturn.PaidAmount += payingAmount;
                break;
            case PurchaseReturnTransactionType.PurchaseReturnUpdate:
            case PurchaseReturnTransactionType.ReturnPaymentUpdate:
                purchaseReturn.PaidAmount += payingAmount;
                break;
            case PurchaseReturnTransactionType.ReturnPaymentDelete:
                purchaseReturn.PaidAmount -= payingAmount;
                break;
            default:
                throw new InvalidOperationException("Unknown transaction type.");
        }

        purchaseReturn.DueAmount = purchaseReturn.GrandTotal - purchaseReturn.PaidAmount;
        await UpdatePurchaseReturnPaymentStatusId(purchaseReturn, cancellationToken);
    }

    public async Task UpdatePurchaseReturnPaymentStatusId(
        PurchaseReturn purchaseReturn,
        CancellationToken cancellationToken = default)
    {
        var paymentStatuses = await commonQueryService
            .GetLookupDetailsAsync((int)LookupDevCode.PurchasePaymentStatus, cancellationToken);

        if (purchaseReturn.GrandTotal == purchaseReturn.PaidAmount)
        {
            purchaseReturn.PaymentStatusId = paymentStatuses.FirstOrDefault(x => x.DevCode == (int)PaymentStatus.Paid)?.Id;
        }
        else if (purchaseReturn.GrandTotal > purchaseReturn.PaidAmount && purchaseReturn.PaidAmount > 0)
        {
            purchaseReturn.PaymentStatusId = paymentStatuses.FirstOrDefault(x => x.DevCode == (int)PaymentStatus.Partial)?.Id;
        }
        else
        {
            purchaseReturn.PaymentStatusId = paymentStatuses.FirstOrDefault(x => x.DevCode == (int)PaymentStatus.Due)?.Id;
        }
    }
}
