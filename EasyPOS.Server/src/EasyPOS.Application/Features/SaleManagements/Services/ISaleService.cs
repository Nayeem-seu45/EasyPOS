using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Domain.Sales;

namespace EasyPOS.Application.Features.SaleManagements.Services;

internal interface ISaleService
{
    Task AdjustSaleAsync(
        SaleTransactionType transactionType,
        Sale sale,
        decimal payingAmount,
        CancellationToken cancellationToken = default);


    void AddPaymentToSale(
        Sale sale,
        decimal receivedAmount,
        decimal payingAmount,
        decimal changeAmount,
        Guid paymentType,
        string? note);
}
