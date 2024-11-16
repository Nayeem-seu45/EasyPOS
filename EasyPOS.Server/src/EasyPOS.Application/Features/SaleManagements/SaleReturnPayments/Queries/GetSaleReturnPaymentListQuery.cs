using EasyPOS.Application.Features.Sales.SaleReturnPayments.Models;

namespace EasyPOS.Application.Features.Sales.SaleReturnPayments.Queries;

[Authorize(Policy = Permissions.SaleReturnPayments.View)]
public record GetSaleReturnPaymentListQuery
     : DataGridModel, ICacheableQuery<PaginatedResponse<SaleReturnPaymentModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.SaleReturnPayment}_{PageNumber}_{PageSize}";
    public Guid SaleReturnId { get; set; }

}

internal sealed class GetSaleReturnPaymentQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetSaleReturnPaymentListQuery, PaginatedResponse<SaleReturnPaymentModel>>
{
    public async Task<Result<PaginatedResponse<SaleReturnPaymentModel>>> Handle(GetSaleReturnPaymentListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(SaleReturnPaymentModel.Id)},
                t.SaleReturnId AS {nameof(SaleReturnPaymentModel.SaleReturnId)},
                t.PaymentDate AS {nameof(SaleReturnPaymentModel.PaymentDate)},
                t.ReceivedAmount AS {nameof(SaleReturnPaymentModel.ReceivedAmount)},
                t.PayingAmount AS {nameof(SaleReturnPaymentModel.PayingAmount)},
                t.ChangeAmount AS {nameof(SaleReturnPaymentModel.ChangeAmount)},
                t.PaymentType AS {nameof(SaleReturnPaymentModel.PaymentType)},
                t.Note AS {nameof(SaleReturnPaymentModel.Note)}
            FROM dbo.SaleReturnPayments AS t
            WHERE 1 = 1
            AND t.SaleReturnId = @SaleReturnId
            """;


        return await PaginatedResponse<SaleReturnPaymentModel>
            .CreateAsync(connection, sql, request, request.SaleReturnId);

    }
}


