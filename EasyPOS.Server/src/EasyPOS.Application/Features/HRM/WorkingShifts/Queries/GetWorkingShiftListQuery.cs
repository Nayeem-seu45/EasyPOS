using EasyPOS.Application.Features.HRM.WorkingShifts.Models;

namespace EasyPOS.Application.Features.HRM.WorkingShifts.Queries;

[Authorize(Policy = Permissions.WorkingShifts.View)]
public record GetWorkingShiftListQuery 
     : DataGridModel, ICacheableQuery<PaginatedResponse<WorkingShiftModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.WorkingShift}_{PageNumber}_{PageSize}";
     
}

internal sealed class GetWorkingShiftQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetWorkingShiftListQuery, PaginatedResponse<WorkingShiftModel>>
{
    public async Task<Result<PaginatedResponse<WorkingShiftModel>>> Handle(GetWorkingShiftListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(WorkingShiftModel.Id)},
                t.ShiftName AS {nameof(WorkingShiftModel.ShiftName)},
                t.Description AS {nameof(WorkingShiftModel.Description)},
                IIF(t.IsActive = 1, 'Active', 'Inactive') AS {nameof(WorkingShiftModel.ActiveStatus)}
            FROM dbo.WorkingShifts AS t
            WHERE 1 = 1
            """;


        return await PaginatedResponse<WorkingShiftModel>
            .CreateAsync(connection, sql, request);

    }
}


