namespace EasyPOS.Application.Features.HRM.Designations.Queries;

public record GetDesignationByIdQuery(Guid Id) : ICacheableQuery<DesignationModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Designation}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetDesignationByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetDesignationByIdQuery, DesignationModel>
{

    public async Task<Result<DesignationModel>> Handle(GetDesignationByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new DesignationModel();
        }
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
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<DesignationModel>(sql, new { request.Id });
    }
}

