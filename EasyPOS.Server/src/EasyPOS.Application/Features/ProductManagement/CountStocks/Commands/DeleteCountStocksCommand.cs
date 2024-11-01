namespace EasyPOS.Application.Features.ProductManagement.CountStocks.Commands;

public record DeleteCountStocksCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.CountStock;
}

internal sealed class DeleteCountStocksCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteCountStocksCommand>

{
    public async Task<Result> Handle(DeleteCountStocksCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.CountStocks
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}