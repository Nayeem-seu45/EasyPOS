namespace EasyPOS.Application.Features.Purchases.PurchaseReturnPayments.Commands;

public record DeletePurchaseReturnPaymentsCommand(Guid[] Ids) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchaseReturnPayment;
}

internal sealed class DeletePurchaseReturnPaymentsCommandHandler(
    ISqlConnectionFactory sqlConnection)
    : ICommandHandler<DeletePurchaseReturnPaymentsCommand>

{
    public async Task<Result> Handle(DeletePurchaseReturnPaymentsCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.PurchaseReturnPayments
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}
