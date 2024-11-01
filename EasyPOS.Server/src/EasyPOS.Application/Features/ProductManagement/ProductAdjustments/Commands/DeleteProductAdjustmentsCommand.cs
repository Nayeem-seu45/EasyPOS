namespace EasyPOS.Application.Features.ProductManagement.ProductAdjustments.Commands;

public record DeleteProductAdjustmentsCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.ProductAdjustment;
}

internal sealed class DeleteProductAdjustmentsCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteProductAdjustmentsCommand>

{
    public async Task<Result> Handle(DeleteProductAdjustmentsCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.ProductAdjustments
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}