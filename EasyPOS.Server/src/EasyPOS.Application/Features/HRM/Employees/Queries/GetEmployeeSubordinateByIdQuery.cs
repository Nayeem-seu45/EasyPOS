namespace EasyPOS.Application.Features.HRM.Employees.Queries;

public record GetEmployeeSubordinateByIdQuery(Guid Id) : ICacheableQuery<HierarchyTreeNodeModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Employee}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;
}

internal sealed class GetEmployeeSubordinateByIdQueryHandler(ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetEmployeeSubordinateByIdQuery, HierarchyTreeNodeModel>
{
    public async Task<Result<HierarchyTreeNodeModel>> Handle(GetEmployeeSubordinateByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return new HierarchyTreeNodeModel();
        }

        var connection = sqlConnection.GetOpenConnection();

        // Recursive function to build the hierarchy
        async Task<HierarchyTreeNodeModel?> BuildHierarchy(Guid employeeId)
        {
            // Query to fetch employee details
            var sql = $"""
            SELECT
                t.Id AS Id,
                t.Code AS Code,
                CONCAT(t.FirstName, ' ', t.LastName) AS FullName,
                t.PhotoUrl AS PhotoUrl,
                ds.Name AS Designation,
                d.Name AS Department
            FROM dbo.Employees AS t
            LEFT JOIN dbo.Departments AS d ON d.Id = t.DepartmentId
            LEFT JOIN dbo.Designations AS ds ON ds.Id = t.DesignationId
            WHERE t.Id = @EmployeeId
            """;

            var employee = await connection.QueryFirstOrDefaultAsync<EmployeeModel>(sql, new { EmployeeId = employeeId });

            if (employee == null) return null;

            // Map the employee details into a node
            var node = new HierarchyTreeNodeModel
            {
                Expanded = true,
                Type = "person",
                Data = new
                {
                    employee.Code,
                    employee.FullName,
                    employee.Designation,
                    employee.Department,
                    employee.PhotoUrl,
                    employee.Id
                },
                Key = employee.Id,
                Children = []
            };

            // Query to fetch subordinates
            var subordinatesSql = $"""
                SELECT Id FROM dbo.Employees WHERE ReportTo = @ManagerId
            """;

            var subordinateIds = await connection.QueryAsync<Guid>(subordinatesSql, new { ManagerId = employee.Id });

            // Recursively build the hierarchy for subordinates
            foreach (var subordinateId in subordinateIds)
            {
                var childNode = await BuildHierarchy(subordinateId);
                if (childNode != null)
                {
                    node.Children.Add(childNode);
                }
            }

            return node;
        }

        // Start building the hierarchy from the requested employee ID
        var rootHierarchy = await BuildHierarchy(request.Id);

        return rootHierarchy ?? new HierarchyTreeNodeModel();
    }
}



