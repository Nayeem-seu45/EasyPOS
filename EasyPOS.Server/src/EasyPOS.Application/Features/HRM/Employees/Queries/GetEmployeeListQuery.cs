namespace EasyPOS.Application.Features.HRM.Employees.Queries;

[Authorize(Policy = Permissions.Employees.View)]
public record GetEmployeeListQuery 
     : DataGridModel, ICacheableQuery<PaginatedResponse<EmployeeModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.Employee}_{PageNumber}_{PageSize}";
     
}

internal sealed class GetEmployeeQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetEmployeeListQuery, PaginatedResponse<EmployeeModel>>
{
    public async Task<Result<PaginatedResponse<EmployeeModel>>> Handle(GetEmployeeListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(EmployeeModel.Id)},
                t.EmployeeCode AS {nameof(EmployeeModel.EmployeeCode)},
                t.EmployeeName AS {nameof(EmployeeModel.EmployeeName)},
                t.Gender AS {nameof(EmployeeModel.Gender)},
                t.NID AS {nameof(EmployeeModel.NID)},
                t.WarehouseId AS {nameof(EmployeeModel.WarehouseId)},
                t.DepartmentId AS {nameof(EmployeeModel.DepartmentId)},
                t.DesignationId AS {nameof(EmployeeModel.DesignationId)},
                t.WorkingShiftId AS {nameof(EmployeeModel.WorkingShiftId)},
                t.Email AS {nameof(EmployeeModel.Email)},
                t.PhoneNo AS {nameof(EmployeeModel.PhoneNo)},
                t.MobileNo AS {nameof(EmployeeModel.MobileNo)},
                t.Country AS {nameof(EmployeeModel.Country)},
                t.City AS {nameof(EmployeeModel.City)},
                t.Address AS {nameof(EmployeeModel.Address)}
            FROM dbo.Employees AS t
            WHERE 1 = 1
            """;


        return await PaginatedResponse<EmployeeModel>
            .CreateAsync(connection, sql, request);

    }
}


