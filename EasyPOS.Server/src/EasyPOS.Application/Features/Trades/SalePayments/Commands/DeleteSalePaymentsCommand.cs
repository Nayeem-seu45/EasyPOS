namespace EasyPOS.Application.Features.Trades.SalePayments.Commands;

public record DeleteSalePaymentsCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SalePayment;
}

internal sealed class DeleteSalePaymentsCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteSalePaymentsCommand>

{
    public async Task<Result> Handle(DeleteSalePaymentsCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.SalePayments
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}
