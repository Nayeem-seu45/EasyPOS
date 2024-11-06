namespace EasyPOS.Application.Features.Sales.GiftCards.Commands;

public record UpdateGiftCardCommand(
    Guid Id,
    string? CardNo,
    decimal? Amount,
    DateTime? ExpiredDate,
    Guid CustomerId,
    bool AllowMultipleTransac
    ) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.GiftCard;
}

internal sealed class UpdateGiftCardCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<UpdateGiftCardCommand>
{
    public async Task<Result> Handle(UpdateGiftCardCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.GiftCards.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
