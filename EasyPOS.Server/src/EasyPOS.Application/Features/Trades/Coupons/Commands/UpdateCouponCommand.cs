namespace EasyPOS.Application.Features.Trades.Coupons.Commands;

public record UpdateCouponCommand(
    Guid Id,
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
    ): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Coupon;
}

internal sealed class UpdateCouponCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateCouponCommand>
{
    public async Task<Result> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Coupons.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}