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
    IApplicationDbContext dbContext)
    : ICommandHandler<CreateSaleReturnCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSaleReturnCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<SaleReturn>();

        dbContext.SaleReturns.Add(entity);
        entity.ReferenceNo = DateTime.Now.ToString("ddMMyyyyhhmmssffff");

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
