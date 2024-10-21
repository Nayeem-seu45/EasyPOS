namespace EasyPOS.Application.Features.Trades.SalePayments.Queries;

[Authorize(Policy = Permissions.SalePayments.View)]
public record GetSalePaymentListQuery
     : DataGridModel, ICacheableQuery<PaginatedResponse<SalePaymentModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.SalePayment}_{PageNumber}_{PageSize}";
    public Guid SaleId { get; set; }

}

internal sealed class GetSalePaymentQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetSalePaymentListQuery, PaginatedResponse<SalePaymentModel>>
{
    public async Task<Result<PaginatedResponse<SalePaymentModel>>> Handle(GetSalePaymentListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(SalePaymentModel.Id)},
                t.SaleId AS {nameof(SalePaymentModel.SaleId)},
                t.PaymentDate AS {nameof(SalePaymentModel.PaymentDate)},
                t.ReceivedAmount AS {nameof(SalePaymentModel.ReceivedAmount)},
                t.PayingAmount AS {nameof(SalePaymentModel.PayingAmount)},
                t.ChangeAmount AS {nameof(SalePaymentModel.ChangeAmount)},
                t.PaymentType AS {nameof(SalePaymentModel.PaymentType)},
                t.Note AS {nameof(SalePaymentModel.Note)}
            FROM dbo.SalePayments AS t
            WHERE 1 = 1
            AND t.SaleId = @SaleId
            """;


        return await PaginatedResponse<SalePaymentModel>
            .CreateAsync(connection, sql, request, request.SaleId);

    }
}


