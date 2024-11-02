namespace EasyPOS.Application.Features.Quotations.Commands;

public record DeleteQuotationsCommand(Guid[] Ids) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Quotation;
}

internal sealed class DeleteQuotationsCommandHandler(
    ISqlConnectionFactory sqlConnection)
    : ICommandHandler<DeleteQuotationsCommand>

{
    public async Task<Result> Handle(DeleteQuotationsCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.Quotations
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}
