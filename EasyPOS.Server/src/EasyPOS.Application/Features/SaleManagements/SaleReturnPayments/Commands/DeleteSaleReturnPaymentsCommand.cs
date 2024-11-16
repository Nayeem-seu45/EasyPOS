namespace EasyPOS.Application.Features.Sales.SaleReturnPayments.Commands;

public record DeleteSaleReturnPaymentsCommand(Guid[] Ids) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SaleReturnPayment;
}

internal sealed class DeleteSaleReturnPaymentsCommandHandler(
    ISqlConnectionFactory sqlConnection)
    : ICommandHandler<DeleteSaleReturnPaymentsCommand>

{
    public async Task<Result> Handle(DeleteSaleReturnPaymentsCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.SaleReturnPayments
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}
