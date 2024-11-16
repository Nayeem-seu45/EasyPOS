namespace EasyPOS.Application.Features.HRM.LeaveTypes.Queries;

public record GetLeaveTypeByIdQuery(Guid Id) : ICacheableQuery<LeaveTypeModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.LeaveType}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetLeaveTypeByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetLeaveTypeByIdQuery, LeaveTypeModel>
{

    public async Task<Result<LeaveTypeModel>> Handle(GetLeaveTypeByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new LeaveTypeModel();
        }
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(LeaveTypeModel.Id)},
                t.Name AS {nameof(LeaveTypeModel.Name)},
                t.Code AS {nameof(LeaveTypeModel.Code)},
                t.TotalLeaveDays AS {nameof(LeaveTypeModel.TotalLeaveDays)},
                t.MaxConsecutiveAllowed AS {nameof(LeaveTypeModel.MaxConsecutiveAllowed)},
                t.IsSandwichAllowed AS {nameof(LeaveTypeModel.IsSandwichAllowed)}
            FROM dbo.LeaveTypes AS t
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<LeaveTypeModel>(sql, new { request.Id });
    }
}

