using EasyPOS.Application.Features.ProductTransfers.Queries;
using EasyPOS.Application.Features.StockManagement.Services;
using EasyPOS.Domain.Common.Enums;
using EasyPOS.Domain.ProductTransfers;

namespace EasyPOS.Application.Features.ProductTransfers.Commands;

public record CreateProductTransferCommand(
    DateOnly TransferDate,
    Guid FromWarehouseId,
    Guid ToWarehouseId,
    Guid TransferStatusId,
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
    List<ProductTransferDetailModel> ProductTransferDetails
    ) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.ProductTransfer;
}

internal sealed class CreateProductTransferCommandHandler(
    IApplicationDbContext dbContext,
    IStockService stockService)
    : ICommandHandler<CreateProductTransferCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductTransferCommand request, CancellationToken cancellationToken)
    {
        // Create the ProductTransfer entity
        var productTransfer = request.Adapt<ProductTransfer>();
        productTransfer.ReferenceNo = UtilityExtensions.GetDateTimeStampRef("PT-");
        dbContext.ProductTransfers.Add(productTransfer);

        // Process product transfer details
        foreach (var detail in request.ProductTransferDetails)
        {
            // Adjust stock for this transfer
            var stockAdjustmentResult = await stockService.AdjustStockOnProductTransferAsync(
                productId: detail.ProductId,
                fromWarehouseId: request.FromWarehouseId,
                toWarehouseId: request.ToWarehouseId,
                quantity: detail.Quantity,
                cancellationToken: cancellationToken);

            if (stockAdjustmentResult.IsFailure)
            {
                // Rollback changes and return error if stock adjustment fails
                return Result.Failure<Guid>(stockAdjustmentResult.Error);
            }
        }

        // Persist all changes in a single SaveChanges call
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(productTransfer.Id);
    }
}

