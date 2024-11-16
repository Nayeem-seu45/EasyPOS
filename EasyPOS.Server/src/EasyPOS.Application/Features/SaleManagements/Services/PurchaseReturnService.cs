using EasyPOS.Application.Common.Enums;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Domain.Sales;

namespace EasyPOS.Application.Features.SaleManagements.Services;

public class SaleReturnService(
    ICommonQueryService commonQueryService) : ISaleReturnService
{
    public async Task AdjustSaleReturnAsync(
        SaleReturnTransactionType transactionType, 
        SaleReturn saleReturn, 
        decimal payingAmount,
        CancellationToken cancellationToken = default)
    {

        switch (transactionType)
        {
            case SaleReturnTransactionType.SaleReturnCreate:
            case SaleReturnTransactionType.ReturnPaymentCreate:
                saleReturn.PaidAmount += payingAmount;
                break;
            case SaleReturnTransactionType.SaleReturnUpdate:
            case SaleReturnTransactionType.ReturnPaymentUpdate:
                saleReturn.PaidAmount += payingAmount;
                break;
            case SaleReturnTransactionType.ReturnPaymentDelete:
                saleReturn.PaidAmount -= payingAmount;
                break;
            default:
                throw new InvalidOperationException("Unknown transaction type.");
        }

        saleReturn.DueAmount = saleReturn.GrandTotal - saleReturn.PaidAmount;
        await UpdateSaleReturnPaymentStatusId(saleReturn, cancellationToken);
    }

    public async Task UpdateSaleReturnPaymentStatusId(
        SaleReturn saleReturn,
        CancellationToken cancellationToken = default)
    {
        var paymentStatuses = await commonQueryService
            .GetLookupDetailsAsync((int)LookupDevCode.SalePaymentStatus, cancellationToken);

        if (saleReturn.GrandTotal == saleReturn.PaidAmount)
        {
            saleReturn.PaymentStatusId = paymentStatuses.FirstOrDefault(x => x.DevCode == (int)SalePaymentStatus.Paid).Id;
        }
        else if (saleReturn.GrandTotal > saleReturn.PaidAmount && saleReturn.PaidAmount > 0)
        {
            saleReturn.PaymentStatusId = paymentStatuses.FirstOrDefault(x => x.DevCode == (int)SalePaymentStatus.Partial).Id;
        }
        else
        {
            saleReturn.PaymentStatusId = paymentStatuses.FirstOrDefault(x => x.DevCode == (int)SalePaymentStatus.Pending).Id;
        }
    }
}
