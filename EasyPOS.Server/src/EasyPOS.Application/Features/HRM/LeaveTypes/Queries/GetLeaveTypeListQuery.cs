namespace EasyPOS.Application.Features.HRM.LeaveTypes.Queries;

[Authorize(Policy = Permissions.LeaveTypes.View)]
public record GetLeaveTypeListQuery 
     : DataGridModel, ICacheableQuery<PaginatedResponse<LeaveTypeModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.LeaveType}_{PageNumber}_{PageSize}";
     
}

internal sealed class GetLeaveTypeQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetLeaveTypeListQuery, PaginatedResponse<LeaveTypeModel>>
{
    public async Task<Result<PaginatedResponse<LeaveTypeModel>>> Handle(GetLeaveTypeListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(LeaveTypeModel.Id)},
                t.Name AS {nameof(LeaveTypeModel.Name)},
                t.Code AS {nameof(LeaveTypeModel.Code)},
                t.TotalLeaveDays AS {nameof(LeaveTypeModel.TotalLeaveDays)},
                t.MaxConsecutiveAllowed AS {nameof(LeaveTypeModel.MaxConsecutiveAllowed)},
                IIF(t.IsSandwichAllowed = 1, 'Yes', 'No') AS {nameof(LeaveTypeModel.SandwichAllowed)}
            FROM dbo.LeaveTypes AS t
            WHERE 1 = 1
            """;


        return await PaginatedResponse<LeaveTypeModel>
            .CreateAsync(connection, sql, request);

    }
}


