using EasyPOS.Application.Features.Sales.SaleReturnPayments.Models;

namespace EasyPOS.Application.Features.Sales.SaleReturnPayments.Queries;

[Authorize(Policy = Permissions.SaleReturnPayments.View)]
public record GetPaymentListBySaleReturnIdQuery
     : ICacheableQuery<List<SaleReturnPaymentModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.SaleReturnPayment}_{SaleReturnId}";
    public Guid SaleReturnId { get; set; }
    public bool? AllowCache => false;
    [JsonIgnore]
    public TimeSpan? Expiration => null;
}

internal sealed class GetPaymentListBySaleReturnIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetPaymentListBySaleReturnIdQuery, List<SaleReturnPaymentModel>>
{
    public async Task<Result<List<SaleReturnPaymentModel>>> Handle(GetPaymentListBySaleReturnIdQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(SaleReturnPaymentModel.Id)},
                t.SaleReturnId AS {nameof(SaleReturnPaymentModel.SaleReturnId)},
                t.PaymentDate AS {nameof(SaleReturnPaymentModel.PaymentDate)},
                -- CAST(t.PaymentDate AS DATE) AS {nameof(SaleReturnPaymentModel.PaymentDateString)},
                FORMAT(t.PaymentDate, 'dd/MM/yyyy') AS {nameof(SaleReturnPaymentModel.PaymentDateString)},
                --FORMAT(t.PaymentDate, 'dd/MM/yyyy HH:mm:ss') AS {nameof(SaleReturnPaymentModel.PaymentDateString)},
                t.ReceivedAmount AS {nameof(SaleReturnPaymentModel.ReceivedAmount)},
                t.PayingAmount AS {nameof(SaleReturnPaymentModel.PayingAmount)},
                t.ChangeAmount AS {nameof(SaleReturnPaymentModel.ChangeAmount)},
                t.PaymentType AS {nameof(SaleReturnPaymentModel.PaymentType)},
                t.Note AS {nameof(SaleReturnPaymentModel.Note)},
                ld.Name AS {nameof(SaleReturnPaymentModel.PaymentTypeName)},
                u.Username AS {nameof(SaleReturnPaymentModel.CreatedBy)}
            FROM dbo.SaleReturnPayments AS t
            LEFT JOIN dbo.LookupDetails ld ON ld.Id = t.PaymentType
            LEFT JOIN [identity].Users u ON u.Id = t.CreatedBy
            WHERE 1 = 1
            AND t.SaleReturnId = @SaleReturnId
            """;


        var result = await connection.QueryAsync<SaleReturnPaymentModel>(sql, new { request.SaleReturnId });
        return result.AsList();

    }
}


