namespace EasyPOS.Application.Features.Trades.GiftCards.Commands;

public record DeleteGiftCardCommand(Guid Id): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.GiftCard;
}

internal sealed class DeleteGiftCardCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteGiftCardCommand>

{
    public async Task<Result> Handle(DeleteGiftCardCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.GiftCards
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.GiftCards.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}