namespace EasyPOS.Application.Features.Sales.Coupons.Commands;

public record DeleteCouponCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Coupon;
}

internal sealed class DeleteCouponCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<DeleteCouponCommand>

{
    public async Task<Result> Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Coupons
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.Coupons.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}