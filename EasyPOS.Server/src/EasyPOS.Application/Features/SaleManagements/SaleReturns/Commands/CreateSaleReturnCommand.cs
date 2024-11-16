using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Application.Features.SaleReturns.Models;
using EasyPOS.Domain.Sales;

namespace EasyPOS.Application.Features.SaleReturns.Commands;

//public record CreateSaleReturnCommand(
//    DateOnly SaleReturnDate,
//    string? ReferenceNo, 
//    Guid WarehouseId, 
//    Guid CustomerId, 
//    Guid BullerId, 
//    string? AttachmentUrl, 
//    Guid SaleReturnStatusId, 
//    Guid PaymentStatusId, 
//    decimal? Tax, 
//    decimal? TaxAmount, 
//    decimal? DiscountAmount, 
//    decimal? ShippingCost, 
//    decimal GrandTotal, 
//    string? SaleReturnNote, 
//    string? StaffNote,
//    List<SaleReturnDetailModel> SaleReturnDetails
//    ) : ICacheInvalidatorCommand<Guid>
//{
//    public string CacheKey => CacheKeys.SaleReturn;
//}

public record CreateSaleReturnCommand : UpsertSaleReturnModel, ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.SaleReturn;
}

internal sealed class CreateSaleReturnCommandHandler(
    IApplicationDbContext dbContext,
    ISaleReturnService saleReturnService)
    : ICommandHandler<CreateSaleReturnCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSaleReturnCommand request, CancellationToken cancellationToken)
    {      
        var saleReturn = request.Adapt<SaleReturn>();

        dbContext.SaleReturns.Add(saleReturn);
        saleReturn.ReferenceNo = "SR-" + DateTime.Now.ToString("yyyyMMddhhmmffff");

        var sale = await dbContext.Sales
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == saleReturn.SaleId);

        if (sale is null)
        {
            return Result.Failure<Guid>(Error.Failure(nameof(sale), "Sale not found"));
        }

        saleReturn.SoldReferenceNo = sale.ReferenceNo;
        saleReturn.WarehouseId = sale.WarehouseId;
        saleReturn.CustomerId = sale.CustomerId;

        await saleReturnService.AdjustSaleReturnAsync(
            SaleReturnTransactionType.SaleReturnCreate, 
            saleReturn, 
            0, 
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return saleReturn.Id;
    }
}
