namespace EasyPOS.Application.Features.HRM.Attendances.Queries;

[Authorize(Policy = Permissions.Attendances.View)]
public record GetAttendanceListQuery 
     : DataGridModel, ICacheableQuery<PaginatedResponse<AttendanceModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.Attendance}_{PageNumber}_{PageSize}";
     
}

internal sealed class GetAttendanceQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetAttendanceListQuery, PaginatedResponse<AttendanceModel>>
{
    public async Task<Result<PaginatedResponse<AttendanceModel>>> Handle(GetAttendanceListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(AttendanceModel.Id)},
                t.EmployeeId AS {nameof(AttendanceModel.EmployeeId)},
                t.AttendanceStatusId AS {nameof(AttendanceModel.AttendanceStatusId)}
            FROM dbo.Attendances AS t
            WHERE 1 = 1
            """;


        return await PaginatedResponse<AttendanceModel>
            .CreateAsync(connection, sql, request);

    }
}


