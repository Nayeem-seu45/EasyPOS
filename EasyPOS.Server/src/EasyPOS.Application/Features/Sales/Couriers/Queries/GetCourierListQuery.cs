namespace EasyPOS.Application.Features.Sales.Couriers.Queries;

[Authorize(Policy = Permissions.Couriers.View)]
public record GetCourierListQuery
     : DataGridModel, ICacheableQuery<PaginatedResponse<CourierModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.Courier}_{PageNumber}_{PageSize}";

}

internal sealed class GetCourierQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetCourierListQuery, PaginatedResponse<CourierModel>>
{
    public async Task<Result<PaginatedResponse<CourierModel>>> Handle(GetCourierListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(CourierModel.Id)},
                t.Name AS {nameof(CourierModel.Name)},
                t.PhoneNo AS {nameof(CourierModel.PhoneNo)},
                t.MobileNo AS {nameof(CourierModel.MobileNo)},
                t.Email AS {nameof(CourierModel.Email)},
                t.Address AS {nameof(CourierModel.Address)}
            FROM dbo.Couriers AS t
            WHERE 1 = 1
            """;


        return await PaginatedResponse<CourierModel>
            .CreateAsync(connection, sql, request);

    }
}


