namespace EasyPOS.Application.Features.Trades.GiftCards.Commands;

public record DeleteGiftCardsCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.GiftCard;
}

internal sealed class DeleteGiftCardsCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteGiftCardsCommand>

{
    public async Task<Result> Handle(DeleteGiftCardsCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.GiftCards
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}