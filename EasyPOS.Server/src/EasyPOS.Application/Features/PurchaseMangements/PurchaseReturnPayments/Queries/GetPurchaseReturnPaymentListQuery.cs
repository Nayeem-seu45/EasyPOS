using EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Models;

namespace EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Queries;

[Authorize(Policy = Permissions.PurchaseReturnPayments.View)]
public record GetPurchaseReturnPaymentListQuery
     : DataGridModel, ICacheableQuery<PaginatedResponse<PurchaseReturnPaymentModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.PurchaseReturnPayment}_{PageNumber}_{PageSize}";
    public Guid PurchaseReturnId { get; set; }

}

internal sealed class GetPurchaseReturnPaymentQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetPurchaseReturnPaymentListQuery, PaginatedResponse<PurchaseReturnPaymentModel>>
{
    public async Task<Result<PaginatedResponse<PurchaseReturnPaymentModel>>> Handle(GetPurchaseReturnPaymentListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(PurchaseReturnPaymentModel.Id)},
                t.PurchaseReturnId AS {nameof(PurchaseReturnPaymentModel.PurchaseReturnId)},
                t.PaymentDate AS {nameof(PurchaseReturnPaymentModel.PaymentDate)},
                t.ReceivedAmount AS {nameof(PurchaseReturnPaymentModel.ReceivedAmount)},
                t.PayingAmount AS {nameof(PurchaseReturnPaymentModel.PayingAmount)},
                t.ChangeAmount AS {nameof(PurchaseReturnPaymentModel.ChangeAmount)},
                t.PaymentType AS {nameof(PurchaseReturnPaymentModel.PaymentType)},
                t.Note AS {nameof(PurchaseReturnPaymentModel.Note)}
            FROM dbo.PurchaseReturnPayments AS t
            WHERE 1 = 1
            AND t.PurchaseReturnId = @PurchaseReturnId
            """;


        return await PaginatedResponse<PurchaseReturnPaymentModel>
            .CreateAsync(connection, sql, request, request.PurchaseReturnId);

    }
}


