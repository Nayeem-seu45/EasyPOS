using EasyPOS.Application.Common.Enums;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Models;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.PurchaseMangements.Services;

public class PurchaseService(
    ICommonQueryService commonQueryService) : IPurchaseService
{
    public async Task UpdatePurchasePaymentFieldsAsync(
        Purchase purchase, 
        decimal payingAmount,
        PurchaseTransactionType transactionType, 
        CancellationToken cancellationToken = default)
    {

        switch (transactionType)
        {
            case PurchaseTransactionType.Payment:
                purchase.PaidAmount += payingAmount;
                break;
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
        purchase.PaymentStatusId = await GetPurchasePaymentId(purchase);
    }

    public async Task<Guid?> GetPurchasePaymentId(
        Purchase purchase, 
        CancellationToken cancellationToken = default)
    {
        var paymentStatuses = await commonQueryService.GetLookupDetailsAsync((int)LookupDevCode.PaymentStatus);

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
}
