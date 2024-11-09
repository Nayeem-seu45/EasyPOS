using EasyPOS.Application.Features.SaleReturns.Models;

namespace EasyPOS.Application.Features.SaleReturns.Commands;

public record UpdateSaleReturnCommand : UpsertSaleReturnModel, ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SaleReturn;
}

internal sealed class UpdateSaleReturnCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<UpdateSaleReturnCommand>
{
    public async Task<Result> Handle(UpdateSaleReturnCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.SaleReturns.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
