namespace EasyPOS.Application.Features.HRM.Employees.Commands;

public record DeleteEmployeesCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Employee;
}

internal sealed class DeleteEmployeesCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteEmployeesCommand>

{
    public async Task<Result> Handle(DeleteEmployeesCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.Employees
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}