using EasyPOS.Application.Common.Enums;
using EasyPOS.Application.Features.Sales.SaleReturnPayments.Models;

namespace EasyPOS.Application.Features.Sales.SaleReturnPayments.Queries;

public record GetSaleReturnPaymentByIdQuery(Guid Id) : ICacheableQuery<SaleReturnPaymentModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.SaleReturnPayment}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;
}

internal sealed class GetSaleReturnPaymentByIdQueryHandler(ISqlConnectionFactory sqlConnection, ICommonQueryService commonQueryService)
     : IQueryHandler<GetSaleReturnPaymentByIdQuery, SaleReturnPaymentModel>
{

    public async Task<Result<SaleReturnPaymentModel>> Handle(GetSaleReturnPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new SaleReturnPaymentModel
            {
                PaymentType = await commonQueryService.GetLookupDetailIdAsync((int)PaymentType.Cach)
            };
        }
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
            WHERE t.Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<SaleReturnPaymentModel>(sql, new { request.Id });
    }
}

