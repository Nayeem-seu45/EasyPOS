﻿using EasyPOS.Application.Features.Trades.Purchases.Queries;
using EasyPOS.Domain.Trades;
using Mapster;

namespace EasyPOS.Application.Features.Trades.Purchases.Commands;

public record CreatePurchaseCommand(
    DateOnly PurchaseDate,
    string ReferenceNo,
    Guid WarehouseId,
    Guid SupplierId,
    Guid PurchaseStatusId,
    string? AttachmentUrl,
    decimal? OrderTax,
    decimal? Discount,
    decimal? ShippingCost,
    string? Note,
    List<PurchaseDetailModel> PurchaseDetails
    ) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Purchase;
}

internal sealed class CreatePurchaseCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<CreatePurchaseCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<Purchase>();

        dbContext.Purchases.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}
