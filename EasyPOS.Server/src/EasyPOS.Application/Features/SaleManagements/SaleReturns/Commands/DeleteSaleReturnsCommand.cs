namespace EasyPOS.Application.Features.SaleReturns.Commands;

public record DeleteSaleReturnsCommand(Guid[] Ids) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SaleReturn;
}

internal sealed class DeleteSaleReturnsCommandHandler(
    ISqlConnectionFactory sqlConnection)
    : ICommandHandler<DeleteSaleReturnsCommand>

{
    public async Task<Result> Handle(DeleteSaleReturnsCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.SaleReturns
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}
