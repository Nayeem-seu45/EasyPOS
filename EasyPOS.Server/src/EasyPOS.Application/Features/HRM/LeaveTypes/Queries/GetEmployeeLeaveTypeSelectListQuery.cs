
namespace EasyPOS.Application.Features.HRM.LeaveTypes.Queries;

public record GetEmployeeLeaveTypeSelectListQuery(Guid EmployeeId, bool CacheAllowed = true)
     : ICacheableQuery<List<LeaveTypeSelectListModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.LeaveType}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => CacheAllowed;
}

internal sealed class GetEmployeeLeaveTypeSelectListQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetEmployeeLeaveTypeSelectListQuery, List<LeaveTypeSelectListModel>>
{
    public async Task<Result<List<LeaveTypeSelectListModel>>> Handle(GetEmployeeLeaveTypeSelectListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(LeaveTypeSelectListModel.Id)},
                t.Name AS {nameof(LeaveTypeSelectListModel.Name)},
                t.Code AS {nameof(LeaveTypeSelectListModel.Code)},
                t.TotalLeaveDays AS {nameof(LeaveTypeSelectListModel.TotalLeaveDays)},
                t.MaxConsecutiveDays AS {nameof(LeaveTypeSelectListModel.MaxConsecutiveDays)},
                t.IsSandwichAllowed AS {nameof(LeaveTypeSelectListModel.IsSandwichAllowed)}
            FROM dbo.LeaveTypes AS t
            LEFT JOIN dbo.EmployeeLeaveTypes AS elt ON elt.LeaveTypeId = t.Id
            WHERE 1 = 1
            AND elt.EmployeeId = @EmployeeId
            """;

        var result = await connection.QueryAsync<LeaveTypeSelectListModel>(sql, new {request.EmployeeId});
        return result.AsList();

    }
}


