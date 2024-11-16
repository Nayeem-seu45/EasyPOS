using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Domain.Sales;

namespace EasyPOS.Application.Features.SaleManagements.Services;

internal interface ISaleReturnService
{
    Task AdjustSaleReturnAsync(
        SaleReturnTransactionType transactionType,
        SaleReturn purchaseReturn,
        decimal payingAmount,
        CancellationToken cancellationToken = default);

    Task UpdateSaleReturnPaymentStatusId(
        SaleReturn purchaseReturn,
        CancellationToken cancellationToken = default);
}
