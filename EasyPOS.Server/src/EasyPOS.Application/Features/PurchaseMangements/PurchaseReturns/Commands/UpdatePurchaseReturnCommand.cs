using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.PurchaseMangements.Shared;
using EasyPOS.Application.Features.PurchaseReturns.Models;
using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Application.Features.PurchaseReturns.Commands;

public record UpdatePurchaseReturnCommand(
    Guid Id,
    DateOnly ReturnDate,
    Guid ReturnStatusId,
    string? AttachmentUrl,
    decimal SubTotal,
    decimal? TaxRate,
    decimal? TaxAmount,
    DiscountType DiscountType,
    decimal? DiscountRate,
    decimal? DiscountAmount,
    decimal? ShippingCost,
    decimal GrandTotal,
    string? Note,
    List<PurchaseReturnDetailModel> PurchaseReturnDetails) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchaseReturn;
}

internal sealed class UpdatePurchaseReturnCommandHandler(
    IApplicationDbContext dbContext,
    IPurchaseReturnService purchaseReturnService)
    : ICommandHandler<UpdatePurchaseReturnCommand>
{
    public async Task<Result> Handle(UpdatePurchaseReturnCommand request, CancellationToken cancellationToken)
    {
        var purchaseReturn = await dbContext.PurchaseReturns.FindAsync(request.Id, cancellationToken);

        if (purchaseReturn is null) return Result.Failure(Error.NotFound(nameof(purchaseReturn), ErrorMessages.EntityNotFound));

        request.Adapt(purchaseReturn);

        await purchaseReturnService.AdjustPurchaseReturnAsync(
            purchaseReturn,
            purchaseReturn.PaidAmount,
            PurchaseReturnTransactionType.PurchaseReturnUpdate,
            cancellationToken);


        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
