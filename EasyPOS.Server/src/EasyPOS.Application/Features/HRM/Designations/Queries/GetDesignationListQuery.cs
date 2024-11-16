namespace EasyPOS.Application.Features.HRM.Designations.Queries;

[Authorize(Policy = Permissions.Designations.View)]
public record GetDesignationListQuery 
     : DataGridModel, ICacheableQuery<PaginatedResponse<DesignationModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.Designation}_{PageNumber}_{PageSize}";
     
}

internal sealed class GetDesignationQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetDesignationListQuery, PaginatedResponse<DesignationModel>>
{
    public async Task<Result<PaginatedResponse<DesignationModel>>> Handle(GetDesignationListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(DesignationModel.Id)},
                t.Name AS {nameof(DesignationModel.Name)},
                t.Code AS {nameof(DesignationModel.Code)},
                t.Description AS {nameof(DesignationModel.Description)},
                t.Status AS {nameof(DesignationModel.Status)},
                t.DepartmentId AS {nameof(DesignationModel.DepartmentId)},
                t.ParentId AS {nameof(DesignationModel.ParentId)}
            FROM dbo.Designations AS t
            WHERE 1 = 1
            """;


        return await PaginatedResponse<DesignationModel>
            .CreateAsync(connection, sql, request);

    }
}


