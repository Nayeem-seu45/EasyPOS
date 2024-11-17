namespace EasyPOS.Application.Features.HRM.Attendances.Queries;

public record GetAttendanceByIdQuery(Guid Id) : ICacheableQuery<AttendanceModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Attendance}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetAttendanceByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetAttendanceByIdQuery, AttendanceModel>
{

    public async Task<Result<AttendanceModel>> Handle(GetAttendanceByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new AttendanceModel();
        }
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(AttendanceModel.Id)},
                t.EmployeeId AS {nameof(AttendanceModel.EmployeeId)},
                t.AttendanceDate AS {nameof(AttendanceModel.AttendanceDate)},
                t.CheckIn AS {nameof(AttendanceModel.CheckIn)},
                t.CheckOut AS {nameof(AttendanceModel.CheckOut)},           
                t.StatusId AS {nameof(AttendanceModel.StatusId)}
            FROM dbo.Attendances AS t
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<AttendanceModel>(sql, new { request.Id });
    }
}

