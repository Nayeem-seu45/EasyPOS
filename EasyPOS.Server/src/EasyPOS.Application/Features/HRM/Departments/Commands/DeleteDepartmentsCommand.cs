namespace EasyPOS.Application.Features.HRM.Departments.Commands;

public record DeleteDepartmentsCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Department;
}

internal sealed class DeleteDepartmentsCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteDepartmentsCommand>

{
    public async Task<Result> Handle(DeleteDepartmentsCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.Departments
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}