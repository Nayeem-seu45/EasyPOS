namespace EasyPOS.Application.Features.HRM.Departments.Queries;

public record GetDepartmentByIdQuery(Guid Id) : ICacheableQuery<DepartmentModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Department}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetDepartmentByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetDepartmentByIdQuery, DepartmentModel>
{

    public async Task<Result<DepartmentModel>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new DepartmentModel();
        }
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(DepartmentModel.Id)},
                t.Name AS {nameof(DepartmentModel.Name)},
                t.Code AS {nameof(DepartmentModel.Code)},
                t.Description AS {nameof(DepartmentModel.Description)},
                t.Status AS {nameof(DepartmentModel.Status)},
                t.ParentId AS {nameof(DepartmentModel.ParentId)},
                t.DepartmentHeadId AS {nameof(DepartmentModel.DepartmentHeadId)}
            FROM dbo.Departments AS t
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<DepartmentModel>(sql, new { request.Id });
    }
}

