namespace EasyPOS.Application.Features.HRM.LeaveRequests.Queries;

public record GetLeaveRequestByIdQuery(Guid Id) : ICacheableQuery<LeaveRequestModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.LeaveRequest}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;
}

internal sealed class GetLeaveRequestByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetLeaveRequestByIdQuery, LeaveRequestModel>
{

    public async Task<Result<LeaveRequestModel>> Handle(GetLeaveRequestByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new LeaveRequestModel();
        }
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(LeaveRequestModel.Id)},
                t.EmployeeId AS {nameof(LeaveRequestModel.EmployeeId)},
                t.LeaveTypeId AS {nameof(LeaveRequestModel.LeaveTypeId)},
                t.StartDate AS {nameof(LeaveRequestModel.StartDate)},
                t.EndDate AS {nameof(LeaveRequestModel.EndDate)},
                t.TotalDays AS {nameof(LeaveRequestModel.TotalDays)},
                t.StatusId AS {nameof(LeaveRequestModel.StatusId)},
                t.AttachmentUrl AS {nameof(LeaveRequestModel.AttachmentUrl)},
                t.Reason AS {nameof(LeaveRequestModel.Reason)}
            FROM dbo.LeaveRequests AS t
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<LeaveRequestModel>(sql, new { request.Id });
    }
}

