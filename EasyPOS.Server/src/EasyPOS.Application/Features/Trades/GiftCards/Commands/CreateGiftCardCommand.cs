using EasyPOS.Domain.Trades;

namespace EasyPOS.Application.Features.Trades.GiftCards.Commands;

public record CreateGiftCardCommand(
    string? CardNo, 
    decimal? Amount, 
    DateTime? ExpiredDate, 
    Guid? CustomerId, 
    bool AllowMultipleTransac, 
    Guid GiftCardType
    ): ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.GiftCard;
}
    
internal sealed class CreateGiftCardCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateGiftCardCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateGiftCardCommand request, CancellationToken cancellationToken)
    {
       var entity = request.Adapt<GiftCard>();

       dbContext.GiftCards.Add(entity);

       await dbContext.SaveChangesAsync(cancellationToken);

       return  entity.Id;
    }
}
