namespace EasyPOS.Application.Features.HRM.Holidays.Queries;

public record GetHolidayByIdQuery(Guid Id) : ICacheableQuery<HolidayModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Holiday}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetHolidayByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetHolidayByIdQuery, HolidayModel>
{

    public async Task<Result<HolidayModel>> Handle(GetHolidayByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new HolidayModel();
        }
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(HolidayModel.Id)},
                t.Title AS {nameof(HolidayModel.Title)},
                t.Description AS {nameof(HolidayModel.Description)},
                t.IsActive AS {nameof(HolidayModel.IsActive)}
            FROM dbo.Holidays AS t
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<HolidayModel>(sql, new { request.Id });
    }
}

