namespace EasyPOS.Application.Features.Accounting.Expenses.Commands;

public record DeleteExpensesCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Expense;
}

internal sealed class DeleteExpensesCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteExpensesCommand>

{
    public async Task<Result> Handle(DeleteExpensesCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.Expenses
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}