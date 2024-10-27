namespace EasyPOS.Application.Features.Trades.Coupons.Commands;

public record DeleteCouponsCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Coupon;
}

internal sealed class DeleteCouponsCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteCouponsCommand>

{
    public async Task<Result> Handle(DeleteCouponsCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.Coupons
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}