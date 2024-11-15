using EasyPOS.Application.Common.Enums;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.PurchaseMangements.Services;

public class PurchaseService(
    ICommonQueryService commonQueryService) : IPurchaseService
{
    public async Task AdjustPurchaseAsync(
        Purchase purchase, 
        decimal payingAmount,
        PurchaseTransactionType transactionType, 
        CancellationToken cancellationToken = default)
    {

        switch (transactionType)
        {
            case PurchaseTransactionType.PurchaseCreate:
            case PurchaseTransactionType.PaymentCreate:
                purchase.PaidAmount += payingAmount;
                break;
            //case PurchaseTransactionType.PurchaseUpdate:
            case PurchaseTransactionType.PaymentUpdate:
                purchase.PaidAmount += payingAmount;
                break;
            case PurchaseTransactionType.PaymentDelete:
                purchase.PaidAmount -= payingAmount;
                break;
            default:
                throw new InvalidOperationException("Unknown transaction type.");
        }

        purchase.DueAmount = purchase.GrandTotal - purchase.PaidAmount;
        await UpdatePurchasePaymentStatusId(purchase, cancellationToken);
    }

    public async Task<Guid?> GetPurchasePaymentId(
        Purchase purchase, 
        CancellationToken cancellationToken = default)
    {
        var paymentStatuses = await commonQueryService
            .GetLookupDetailsAsync((int)LookupDevCode.PurchasePaymentStatus, cancellationToken);

        if (purchase.GrandTotal == purchase.PaidAmount)
        {
            return paymentStatuses.FirstOrDefault(x => x.DevCode == (int)PaymentStatus.Paid)?.Id;
        }
        else if (purchase.GrandTotal > purchase.PaidAmount && purchase.PaidAmount > 0)
        {
            return paymentStatuses.FirstOrDefault(x => x.DevCode == (int)PaymentStatus.Partial)?.Id;
        }
        else
        {
            return paymentStatuses.FirstOrDefault(x => x.DevCode == (int)PaymentStatus.Due)?.Id;
        }
    }

    public async Task UpdatePurchasePaymentStatusId(
        Purchase purchase,
        CancellationToken cancellationToken = default)
    {
        var paymentStatuses = await commonQueryService
            .GetLookupDetailsAsync((int)LookupDevCode.PurchasePaymentStatus, cancellationToken);

        if (purchase.GrandTotal == purchase.PaidAmount)
        {
            purchase.PaymentStatusId = paymentStatuses.FirstOrDefault(x => x.DevCode == (int)PaymentStatus.Paid)?.Id;
        }
        else if (purchase.GrandTotal > purchase.PaidAmount && purchase.PaidAmount > 0)
        {
            purchase.PaymentStatusId = paymentStatuses.FirstOrDefault(x => x.DevCode == (int)PaymentStatus.Partial)?.Id;
        }
        else
        {
            purchase.PaymentStatusId = paymentStatuses.FirstOrDefault(x => x.DevCode == (int)PaymentStatus.Due)?.Id;
        }
    }
}
