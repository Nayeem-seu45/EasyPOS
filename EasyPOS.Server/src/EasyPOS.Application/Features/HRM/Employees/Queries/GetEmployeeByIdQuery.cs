namespace EasyPOS.Application.Features.HRM.Employees.Queries;

public record GetEmployeeByIdQuery(Guid Id) : ICacheableQuery<EmployeeModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Employee}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;
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
                t.Address AS {nameof(EmployeeModel.Address)},
                elt.LeaveTypeId
            FROM dbo.Employees AS t
            LEFT JOIN dbo.EmployeeLeaveTypes AS elt ON elt.EmployeeId = t.Id
            WHERE t.Id = @Id
            """;

        var employeeDictionary = new Dictionary<Guid, EmployeeModel>();

        var result = await connection.QueryAsync<EmployeeModel, Guid, EmployeeModel>(
            sql,
            (employee, leaveTypeId) =>
            {
                if (!employeeDictionary.TryGetValue(employee.Id, out var employeeEntry))
                {
                    employeeEntry = employee;
                    employeeEntry.LeaveTypes = [];
                    employeeDictionary[employee.Id] = employeeEntry;
                }

                if (leaveTypeId != Guid.Empty)
                {
                    employeeEntry.LeaveTypes.Add(leaveTypeId);
                }

                return employeeEntry;
            },
            new { request.Id },
            splitOn: "LeaveTypeId"
        );

        var employee = employeeDictionary.Values.FirstOrDefault();

        return employee;
    }
}

