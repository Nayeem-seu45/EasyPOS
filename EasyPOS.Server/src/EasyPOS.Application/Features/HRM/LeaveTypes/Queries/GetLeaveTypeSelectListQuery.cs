
namespace EasyPOS.Application.Features.HRM.LeaveTypes.Queries;

public record GetLeaveTypeSelectListQuery(bool CacheAllowed = true)
     : ICacheableQuery<List<LeaveTypeSelectListModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.LeaveType}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => CacheAllowed;
}

internal sealed class GetLeaveTypeSelectListQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetLeaveTypeSelectListQuery, List<LeaveTypeSelectListModel>>
{
    public async Task<Result<List<LeaveTypeSelectListModel>>> Handle(GetLeaveTypeSelectListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(LeaveTypeSelectListModel.Id)},
                t.Name AS {nameof(LeaveTypeSelectListModel.Name)},
                t.Code AS {nameof(LeaveTypeSelectListModel.Code)},
                t.TotalLeaveDays AS {nameof(LeaveTypeSelectListModel.TotalLeaveDays)},
                t.MaxConsecutiveAllowed AS {nameof(LeaveTypeSelectListModel.MaxConsecutiveAllowed)},
                t.IsSandwichAllowed AS {nameof(LeaveTypeSelectListModel.IsSandwichAllowed)}
            FROM dbo.LeaveTypes AS t
            WHERE 1 = 1
            """;

        var result = await connection.QueryAsync<LeaveTypeSelectListModel>(sql);
        return result.AsList();

    }
}


