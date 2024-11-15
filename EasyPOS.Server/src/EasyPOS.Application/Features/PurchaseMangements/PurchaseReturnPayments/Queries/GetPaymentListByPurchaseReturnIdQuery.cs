using EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Models;

namespace EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Queries;

[Authorize(Policy = Permissions.PurchaseReturnPayments.View)]
public record GetPaymentListByPurchaseReturnIdQuery
     : ICacheableQuery<List<PurchaseReturnPaymentModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.PurchaseReturnPayment}_{PurchaseReturnId}";
    public Guid PurchaseReturnId { get; set; }
    public bool? AllowCache => false;
    [JsonIgnore]
    public TimeSpan? Expiration => null;
}

internal sealed class GetPaymentListByPurchaseReturnIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetPaymentListByPurchaseReturnIdQuery, List<PurchaseReturnPaymentModel>>
{
    public async Task<Result<List<PurchaseReturnPaymentModel>>> Handle(GetPaymentListByPurchaseReturnIdQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(PurchaseReturnPaymentModel.Id)},
                t.PurchaseReturnId AS {nameof(PurchaseReturnPaymentModel.PurchaseReturnId)},
                t.PaymentDate AS {nameof(PurchaseReturnPaymentModel.PaymentDate)},
                -- CAST(t.PaymentDate AS DATE) AS {nameof(PurchaseReturnPaymentModel.PaymentDateString)},
                FORMAT(t.PaymentDate, 'dd/MM/yyyy') AS {nameof(PurchaseReturnPaymentModel.PaymentDateString)},
                --FORMAT(t.PaymentDate, 'dd/MM/yyyy HH:mm:ss') AS {nameof(PurchaseReturnPaymentModel.PaymentDateString)},
                t.ReceivedAmount AS {nameof(PurchaseReturnPaymentModel.ReceivedAmount)},
                t.PayingAmount AS {nameof(PurchaseReturnPaymentModel.PayingAmount)},
                t.ChangeAmount AS {nameof(PurchaseReturnPaymentModel.ChangeAmount)},
                t.PaymentType AS {nameof(PurchaseReturnPaymentModel.PaymentType)},
                t.Note AS {nameof(PurchaseReturnPaymentModel.Note)},
                ld.Name AS {nameof(PurchaseReturnPaymentModel.PaymentTypeName)},
                u.Username AS {nameof(PurchaseReturnPaymentModel.CreatedBy)}
            FROM dbo.PurchaseReturnPayments AS t
            LEFT JOIN dbo.LookupDetails ld ON ld.Id = t.PaymentType
            LEFT JOIN [identity].Users u ON u.Id = t.CreatedBy
            WHERE 1 = 1
            AND t.PurchaseReturnId = @PurchaseReturnId
            """;


        var result = await connection.QueryAsync<PurchaseReturnPaymentModel>(sql, new { request.PurchaseReturnId });
        return result.AsList();

    }
}


