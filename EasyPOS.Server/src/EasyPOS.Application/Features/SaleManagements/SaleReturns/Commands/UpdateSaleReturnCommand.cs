using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Application.Features.SaleReturns.Models;

namespace EasyPOS.Application.Features.SaleReturns.Commands;

public record UpdateSaleReturnCommand : UpsertSaleReturnModel, ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SaleReturn;
}

internal sealed class UpdateSaleReturnCommandHandler(
    IApplicationDbContext dbContext,
    ISaleReturnService saleReturnService)
    : ICommandHandler<UpdateSaleReturnCommand>
{
    public async Task<Result> Handle(UpdateSaleReturnCommand request, CancellationToken cancellationToken)
    {
        var saleReturn = await dbContext.SaleReturns.FindAsync([request.Id], cancellationToken);

        if (saleReturn is null) return Result.Failure(Error.NotFound(nameof(saleReturn), ErrorMessages.EntityNotFound));

        request.Adapt(saleReturn);

        await saleReturnService.AdjustSaleReturnAsync(
            SaleReturnTransactionType.SaleReturnUpdate,
            saleReturn,
            saleReturn.PaidAmount,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
