using EasyPOS.Application.Common.Enums;

namespace EasyPOS.Application.Features.Sales.SalePayments.Queries;

public record GetSalePaymentByIdQuery(Guid Id) : ICacheableQuery<SalePaymentModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.SalePayment}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;
}

internal sealed class GetSalePaymentByIdQueryHandler(ISqlConnectionFactory sqlConnection, ICommonQueryService commonQueryService)
     : IQueryHandler<GetSalePaymentByIdQuery, SalePaymentModel>
{

    public async Task<Result<SalePaymentModel>> Handle(GetSalePaymentByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new SalePaymentModel
            {
                PaymentType = await commonQueryService.GetLookupDetailIdAsync((int)PaymentType.Cach)
            };
        }
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
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<SalePaymentModel>(sql, new { request.Id });
    }
}

