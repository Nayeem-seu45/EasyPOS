namespace EasyPOS.Application.Features.Trades.Couriers.Queries;

public record GetCourierByIdQuery(Guid Id) : ICacheableQuery<CourierModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Courier}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetCourierByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetCourierByIdQuery, CourierModel>
{

    public async Task<Result<CourierModel>> Handle(GetCourierByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new CourierModel();
        }
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
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<CourierModel>(sql, new { request.Id });
    }
}

