namespace EasyPOS.Application.Features.HRM.WorkingShifts.Queries;

public record GetWorkingShiftByIdQuery(Guid Id) : ICacheableQuery<WorkingShiftModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.WorkingShift}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetWorkingShiftByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetWorkingShiftByIdQuery, WorkingShiftModel>
{

    public async Task<Result<WorkingShiftModel>> Handle(GetWorkingShiftByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new WorkingShiftModel();
        }
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(WorkingShiftModel.Id)},
                t.ShiftName AS {nameof(WorkingShiftModel.ShiftName)},
                t.Description AS {nameof(WorkingShiftModel.Description)},
                t.IsActive AS {nameof(WorkingShiftModel.IsActive)}
            FROM dbo.WorkingShifts AS t
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<WorkingShiftModel>(sql, new { request.Id });
    }
}

