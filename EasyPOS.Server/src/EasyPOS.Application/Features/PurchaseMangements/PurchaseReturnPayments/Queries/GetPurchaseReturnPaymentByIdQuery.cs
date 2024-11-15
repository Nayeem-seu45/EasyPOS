using EasyPOS.Application.Common.Enums;
using EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Models;

namespace EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Queries;

public record GetPurchaseReturnPaymentByIdQuery(Guid Id) : ICacheableQuery<PurchaseReturnPaymentModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.PurchaseReturnPayment}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;
}

internal sealed class GetPurchaseReturnPaymentByIdQueryHandler(ISqlConnectionFactory sqlConnection, ICommonQueryService commonQueryService)
     : IQueryHandler<GetPurchaseReturnPaymentByIdQuery, PurchaseReturnPaymentModel>
{

    public async Task<Result<PurchaseReturnPaymentModel>> Handle(GetPurchaseReturnPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new PurchaseReturnPaymentModel
            {
                PaymentType = await commonQueryService.GetLookupDetailIdAsync((int)PaymentType.Cach)
            };
        }
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
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<PurchaseReturnPaymentModel>(sql, new { request.Id });
    }
}

