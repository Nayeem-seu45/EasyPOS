namespace EasyPOS.Application.Features.HRM.Departments.Queries;

[Authorize(Policy = Permissions.Departments.View)]
public record GetDepartmentListQuery 
     : DataGridModel, ICacheableQuery<PaginatedResponse<DepartmentModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.Department}_{PageNumber}_{PageSize}";
     
}

internal sealed class GetDepartmentQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetDepartmentListQuery, PaginatedResponse<DepartmentModel>>
{
    public async Task<Result<PaginatedResponse<DepartmentModel>>> Handle(GetDepartmentListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(DepartmentModel.Id)},
                t.Name AS {nameof(DepartmentModel.Name)},
                t.Code AS {nameof(DepartmentModel.Code)},
                t.Description AS {nameof(DepartmentModel.Description)},
                t.Status AS {nameof(DepartmentModel.Status)},
                t.ParentId AS {nameof(DepartmentModel.ParentId)},
                t.DepartmentHeadId AS {nameof(DepartmentModel.DepartmentHeadId)},
                Concat(dh.FirstName, ' ', dh.LastName) AS {nameof(DepartmentModel.DepartmentHead)},
                IIF(t.Status = 1, 'Active', 'Inactive') AS {nameof(DepartmentModel.ActiveStatus)}
            FROM dbo.Departments AS t
            LEFT JOIN dbo.Employees AS dh ON dh.Id = t.DepartmentHeadId
            WHERE 1 = 1
            """;


        return await PaginatedResponse<DepartmentModel>
            .CreateAsync(connection, sql, request);

    }
}


