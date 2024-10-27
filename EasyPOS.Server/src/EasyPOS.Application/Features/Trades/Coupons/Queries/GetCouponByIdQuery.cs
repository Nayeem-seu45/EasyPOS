namespace EasyPOS.Application.Features.Trades.Coupons.Queries;

public record GetCouponByIdQuery(Guid Id) : ICacheableQuery<CouponModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Coupon}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetCouponByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetCouponByIdQuery, CouponModel>
{

    public async Task<Result<CouponModel>> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new CouponModel();
        }
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(CouponModel.Id)},
                t.Code AS {nameof(CouponModel.Code)},
                t.Name AS {nameof(CouponModel.Name)},
                t.Description AS {nameof(CouponModel.Description)},
                t.DiscountType AS {nameof(CouponModel.DiscountType)},
                t.Amount AS {nameof(CouponModel.Amount)},
                t.ExpiryDate AS {nameof(CouponModel.ExpiryDate)},
                t.AllowFreeShipping AS {nameof(CouponModel.AllowFreeShipping)},
                t.MinimumSpend AS {nameof(CouponModel.MinimumSpend)},
                t.MaximumSpend AS {nameof(CouponModel.MaximumSpend)},
                t.OnlyIndivisual AS {nameof(CouponModel.OnlyIndivisual)},
                t.PerCouponUsageLimit AS {nameof(CouponModel.PerCouponUsageLimit)},
                t.PerUserUsageLimit AS {nameof(CouponModel.PerUserUsageLimit)}
            FROM dbo.Coupons AS t
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<CouponModel>(sql, new { request.Id });
    }
}

