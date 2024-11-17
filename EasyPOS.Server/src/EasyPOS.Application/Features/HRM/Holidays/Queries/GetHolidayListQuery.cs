namespace EasyPOS.Application.Features.HRM.Holidays.Queries;

[Authorize(Policy = Permissions.Holidays.View)]
public record GetHolidayListQuery 
     : DataGridModel, ICacheableQuery<PaginatedResponse<HolidayModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.Holiday}_{PageNumber}_{PageSize}";
     
}

internal sealed class GetHolidayQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetHolidayListQuery, PaginatedResponse<HolidayModel>>
{
    public async Task<Result<PaginatedResponse<HolidayModel>>> Handle(GetHolidayListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(HolidayModel.Id)},
                t.Title AS {nameof(HolidayModel.Title)},
                t.StartDate AS {nameof(HolidayModel.StartDate)},
                t.EndDate AS {nameof(HolidayModel.EndDate)},
                t.Description AS {nameof(HolidayModel.Description)},
                IIF(t.IsActive = 1, 'Active', 'Inactive') AS {nameof(HolidayModel.ActiveStatus)}
            FROM dbo.Holidays AS t
            WHERE 1 = 1
            """;


        return await PaginatedResponse<HolidayModel>
            .CreateAsync(connection, sql, request);

    }
}


