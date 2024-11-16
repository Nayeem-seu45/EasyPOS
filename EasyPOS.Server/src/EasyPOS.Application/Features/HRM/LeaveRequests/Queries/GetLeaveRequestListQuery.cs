namespace EasyPOS.Application.Features.HRM.LeaveRequests.Queries;

[Authorize(Policy = Permissions.LeaveRequests.View)]
public record GetLeaveRequestListQuery 
     : DataGridModel, ICacheableQuery<PaginatedResponse<LeaveRequestModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.LeaveRequest}_{PageNumber}_{PageSize}";
     
}

internal sealed class GetLeaveRequestQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetLeaveRequestListQuery, PaginatedResponse<LeaveRequestModel>>
{
    public async Task<Result<PaginatedResponse<LeaveRequestModel>>> Handle(GetLeaveRequestListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(LeaveRequestModel.Id)},
                t.TotalDays AS {nameof(LeaveRequestModel.TotalDays)},
                t.EmployeeId AS {nameof(LeaveRequestModel.EmployeeId)},
                t.LeaveTypeId AS {nameof(LeaveRequestModel.LeaveTypeId)},
                t.StatusId AS {nameof(LeaveRequestModel.StatusId)},
                t.AttachmentUrl AS {nameof(LeaveRequestModel.AttachmentUrl)},
                t.Reason AS {nameof(LeaveRequestModel.Reason)}
            FROM dbo.LeaveRequests AS t
            WHERE 1 = 1
            """;


        return await PaginatedResponse<LeaveRequestModel>
            .CreateAsync(connection, sql, request);

    }
}


