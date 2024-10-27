namespace EasyPOS.Application.Features.Trades.Couriers.Commands;

public record DeleteCouriersCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Courier;
}

internal sealed class DeleteCouriersCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteCouriersCommand>

{
    public async Task<Result> Handle(DeleteCouriersCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.Couriers
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}