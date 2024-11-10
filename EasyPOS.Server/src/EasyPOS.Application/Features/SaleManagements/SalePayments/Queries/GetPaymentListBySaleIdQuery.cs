namespace EasyPOS.Application.Features.Sales.SalePayments.Queries;

[Authorize(Policy = Permissions.SalePayments.View)]
public record GetPaymentListBySaleIdQuery
     : ICacheableQuery<List<SalePaymentModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.SalePayment}_{SaleId}";
    public Guid SaleId { get; set; }
    public bool? AllowCache => false;
    [JsonIgnore]
    public TimeSpan? Expiration => null;
}

internal sealed class GetPaymentListBySaleIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetPaymentListBySaleIdQuery, List<SalePaymentModel>>
{
    public async Task<Result<List<SalePaymentModel>>> Handle(GetPaymentListBySaleIdQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(SalePaymentModel.Id)},
                t.SaleId AS {nameof(SalePaymentModel.SaleId)},
                t.PaymentDate AS {nameof(SalePaymentModel.PaymentDate)},
                -- CAST(t.PaymentDate AS DATE) AS {nameof(SalePaymentModel.PaymentDateString)},
                FORMAT(t.PaymentDate, 'dd/MM/yyyy') AS {nameof(SalePaymentModel.PaymentDateString)},
                --FORMAT(t.PaymentDate, 'dd/MM/yyyy HH:mm:ss') AS {nameof(SalePaymentModel.PaymentDateString)},
                t.ReceivedAmount AS {nameof(SalePaymentModel.ReceivedAmount)},
                t.PayingAmount AS {nameof(SalePaymentModel.PayingAmount)},
                t.ChangeAmount AS {nameof(SalePaymentModel.ChangeAmount)},
                t.PaymentType AS {nameof(SalePaymentModel.PaymentType)},
                t.Note AS {nameof(SalePaymentModel.Note)},
                ld.Name AS {nameof(SalePaymentModel.PaymentTypeName)},
                u.Username AS {nameof(SalePaymentModel.CreatedBy)}
            FROM dbo.SalePayments AS t
            LEFT JOIN dbo.LookupDetails ld ON ld.Id = t.PaymentType
            LEFT JOIN [identity].Users u ON u.Id = t.CreatedBy
            WHERE 1 = 1
            AND t.SaleId = @SaleId
            """;


        var result = await connection.QueryAsync<SalePaymentModel>(sql, new { request.SaleId });
        return result.AsList();

    }
}


