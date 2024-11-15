using EasyPOS.Application.Common.Enums;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Domain.Sales;

namespace EasyPOS.Application.Features.SaleManagements.Services;
public class SaleService(
    ICommonQueryService commonQueryService) : ISaleService
{

    public async Task AdjustSaleAsync(
        SaleTransactionType transactionType,
        Sale sale,
        decimal payingAmount,
        CancellationToken cancellationToken = default)
    {
        // Adjust paid and due amounts based on transaction type
        switch (transactionType)
        {
            case SaleTransactionType.SaleCreate:
            case SaleTransactionType.PaymentCreate:
                sale.PaidAmount += payingAmount;
                break;
            case SaleTransactionType.SaleUpdate:
            case SaleTransactionType.PaymentUpdate:
                sale.PaidAmount += payingAmount;
                break;
            case SaleTransactionType.PaymentDelete:
                sale.PaidAmount -= payingAmount;
                break;
            default:
                throw new InvalidOperationException("Unknown transaction type.");
        }

        sale.DueAmount = sale.GrandTotal - sale.PaidAmount;
        await UpdateSalePaymentStatusIdAsync(sale, cancellationToken);
    }

    public void AddPaymentToSale(
        Sale sale,
        decimal receivedAmount,
        decimal payingAmount,
        decimal changeAmount,
        Guid paymentType,
        string? note)
    {
        sale.SalePayments.Add(new SalePayment
        {
            ReceivedAmount = receivedAmount,
            PayingAmount = payingAmount,
            ChangeAmount = changeAmount,
            PaymentType = paymentType,
            Note = note,
            PaymentDate = DateTime.Now
        });
    }


    private async Task UpdateSalePaymentStatusIdAsync(Sale sale, CancellationToken cancellationToken)
    {
        var paymentStatuses = await commonQueryService.GetLookupDetailsAsync((int)LookupDevCode.SalePaymentStatus, cancellationToken);

        if (sale.DueAmount == 0) sale.PaymentStatusId = paymentStatuses.FirstOrDefault(x => x.DevCode == (int)SalePaymentStatus.Paid).Id;
        else if (sale.PaidAmount > 0) sale.PaymentStatusId = paymentStatuses.FirstOrDefault(x => x.DevCode == (int)SalePaymentStatus.Partial).Id;
        else sale.PaymentStatusId = paymentStatuses.FirstOrDefault(x => x.DevCode == (int)SalePaymentStatus.Pending).Id;
    }
}
