using EasyPOS.Domain.Trades;

namespace EasyPOS.Application.Features.Trades.Coupons.Commands;

public record CreateCouponCommand(
    string? Code, 
    string Name, 
    string? Description, 
    int DiscountType, 
    decimal Amount, 
    DateTime ExpiryDate, 
    bool AllowFreeShipping, 
    decimal? MinimumSpend, 
    decimal? MaximumSpend, 
    bool OnlyIndivisual, 
    decimal? PerCouponUsageLimit, 
    decimal? PerUserUsageLimit
    ): ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Coupon;
}
    
internal sealed class CreateCouponCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateCouponCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
       var entity = request.Adapt<Coupon>();

       dbContext.Coupons.Add(entity);

       await dbContext.SaveChangesAsync(cancellationToken);

       return  entity.Id;
    }
}