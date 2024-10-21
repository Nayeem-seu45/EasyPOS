using EasyPOS.Application.Common.Enums;
using EasyPOS.Domain.Trades;

namespace EasyPOS.Application.Features.Trades.Sales.Shared;

internal static class SaleSharedService
{
    public static async Task<Guid?> GetSalePaymentId(ICommonQueryService commonQueryService, Sale sale)
    {
        var paymentStatuses = await commonQueryService.GetLookupDetailsAsync((int)LookupDevCode.PaymentStatus);

        if (sale.GrandTotal == sale.PaidAmount)
        {
            return paymentStatuses.FirstOrDefault(x => x.DevCode == (int)PaymentStatus.Paid)?.Id;
        }
        else if (sale.GrandTotal > sale.PaidAmount && sale.PaidAmount > 0)
        {
            return paymentStatuses.FirstOrDefault(x => x.DevCode == (int)PaymentStatus.Partial)?.Id;
        }
        else
        {
            return paymentStatuses.FirstOrDefault(x => x.DevCode == (int)PaymentStatus.Due)?.Id;
        }
    }
}
