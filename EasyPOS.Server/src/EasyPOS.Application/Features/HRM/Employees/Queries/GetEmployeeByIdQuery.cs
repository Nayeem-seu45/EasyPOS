namespace EasyPOS.Application.Features.HRM.Employees.Queries;

public record GetEmployeeByIdQuery(Guid Id) : ICacheableQuery<EmployeeModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Employee}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetEmployeeByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetEmployeeByIdQuery, EmployeeModel>
{

    public async Task<Result<EmployeeModel>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new EmployeeModel();
        }
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(EmployeeModel.Id)},
                t.Code AS {nameof(EmployeeModel.Code)},
                t.FirstName AS {nameof(EmployeeModel.FirstName)},
                t.LastName AS {nameof(EmployeeModel.LastName)},
                t.Gender AS {nameof(EmployeeModel.Gender)},
                t.NID AS {nameof(EmployeeModel.NID)},
                t.DOB AS {nameof(EmployeeModel.DOB)},
                t.WarehouseId AS {nameof(EmployeeModel.WarehouseId)},
                t.DepartmentId AS {nameof(EmployeeModel.DepartmentId)},
                t.DesignationId AS {nameof(EmployeeModel.DesignationId)},
                t.WorkingShiftId AS {nameof(EmployeeModel.WorkingShiftId)},
                t.ReportTo AS {nameof(EmployeeModel.ReportTo)},
                t.Email AS {nameof(EmployeeModel.Email)},
                t.PhoneNo AS {nameof(EmployeeModel.PhoneNo)},
                t.MobileNo AS {nameof(EmployeeModel.MobileNo)},
                t.Country AS {nameof(EmployeeModel.Country)},
                t.City AS {nameof(EmployeeModel.City)},
                t.Address AS {nameof(EmployeeModel.Address)}
            FROM dbo.Employees AS t
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<EmployeeModel>(sql, new { request.Id });
    }
}

