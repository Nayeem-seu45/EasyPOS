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
                t.Email AS {nameof(EmployeeModel.Email)},
                t.PhoneNo AS {nameof(EmployeeModel.PhoneNo)},
                t.MobileNo AS {nameof(EmployeeModel.MobileNo)},
                t.Country AS {nameof(EmployeeModel.Country)},
                t.City AS {nameof(EmployeeModel.City)},
                t.Address AS {nameof(EmployeeModel.Address)},

                w.Name AS {nameof(EmployeeModel.Warehouse)},
                d.Name AS {nameof(EmployeeModel.Department)},
                ds.Name AS {nameof(EmployeeModel.Designation)},
                ws.ShiftName AS {nameof(EmployeeModel.WorkingShift)}
            FROM dbo.Employees AS t
            LEFT JOIN dbo.Warehouses AS w ON w.Id = t.WarehouseId
            LEFT JOIN dbo.Departments AS d ON d.Id = t.DepartmentId
            LEFT JOIN dbo.Designations AS ds ON ds.Id = t.DesignationId
            LEFT JOIN dbo.WorkingShifts AS ws ON ws.Id = t.WorkingShiftId
            WHERE 1 = 1
            """;


        return await PaginatedResponse<EmployeeModel>
            .CreateAsync(connection, sql, request);

    }
}


