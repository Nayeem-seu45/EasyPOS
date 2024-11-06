namespace EasyPOS.Application.Features.Sales.Coupons.Queries;

[Authorize(Policy = Permissions.Coupons.View)]
public record GetCouponListQuery
     : DataGridModel, ICacheableQuery<PaginatedResponse<CouponModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.Coupon}_{PageNumber}_{PageSize}";

}

internal sealed class GetCouponQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetCouponListQuery, PaginatedResponse<CouponModel>>
{
    public async Task<Result<PaginatedResponse<CouponModel>>> Handle(GetCouponListQuery request, CancellationToken cancellationToken)
    {
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
            WHERE 1 = 1
            """;


        return await PaginatedResponse<CouponModel>
            .CreateAsync(connection, sql, request);

    }
}


