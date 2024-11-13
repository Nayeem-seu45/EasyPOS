using EasyPOS.Application.Common.Enums;
using EasyPOS.Application.Features.Sales.Models;
using EasyPOS.Domain.Common;
using EasyPOS.Domain.Sales;

namespace EasyPOS.Application.Features.Sales.Commands;

//public record CreateSaleCommand(
//    DateOnly SaleDate,
//    string? ReferenceNo, 
//    Guid WarehouseId, 
//    Guid CustomerId, 
//    Guid BullerId, 
//    string? AttachmentUrl, 
//    Guid SaleStatusId, 
//    Guid PaymentStatusId, 
//    decimal? Tax, 
//    decimal? TaxAmount, 
//    decimal? DiscountAmount, 
//    decimal? ShippingCost, 
//    decimal GrandTotal, 
//    string? SaleNote, 
//    string? StaffNote,
//    List<SaleDetailModel> SaleDetails
//    ) : ICacheInvalidatorCommand<Guid>
//{
//    public string CacheKey => CacheKeys.Sale;
//}

public record CreateSaleCommand : UpsertSaleModel, ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Sale;
}

internal sealed class CreateSaleCommandHandler(
    IApplicationDbContext dbContext,
    ICommonQueryService commonQueryService)
    : ICommandHandler<CreateSaleCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<Sale>();

        dbContext.Sales.Add(entity);

        var salePaymentStatusList = await commonQueryService
            .GetLookupDetailsAsync((int)LookupDevCode.SalePaymentStatus, cancellationToken);

        if (entity.GrandTotal == request?.SalePayment?.PayingAmount)
        {
            entity.PaymentStatusId = GetSalePaymentStatusId(salePaymentStatusList, SalePaymentStatus.Paid);
        }
        else if (entity.GrandTotal > request?.SalePayment?.PayingAmount && request?.SalePayment?.PayingAmount > 0)
        {
            entity.PaymentStatusId = GetSalePaymentStatusId(salePaymentStatusList, SalePaymentStatus.Partial);
        }
        else if (request?.SalePayment?.PayingAmount == 0)
        {
            entity.PaymentStatusId = GetSalePaymentStatusId(salePaymentStatusList, SalePaymentStatus.Pending);
        }
        entity.DueAmount = entity.GrandTotal - request.SalePayment.PayingAmount;
        entity.PaidAmount = request.SalePayment.PayingAmount;


        // Customer
        var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.Id == entity.CustomerId);

        if(customer is null)
        {
            return Result.Failure<Guid>(Error.Failure(nameof(customer), "Customer not found."));
        }

        customer.TotalDueAmount += entity.GrandTotal - request.SalePayment.PayingAmount;
        customer.TotalPaidAmount += request.SalePayment.PayingAmount;


        // Payment
        if(request.SalePayment is not null
            && request.SalePayment?.PayingAmount > 0
            && !request.SalePayment.PaymentType.IsNullOrEmpty())
        {
            entity.SalePayments.Add(new SalePayment
            {
                ReceivedAmount = request.SalePayment.ReceivedAmount,
                PayingAmount = request.SalePayment.PayingAmount,
                ChangeAmount = request.SalePayment.ChangeAmount,
                PaymentType = request.SalePayment.PaymentType.Value,
                Note = request.SalePayment.Note,
                PaymentDate = DateTime.Now
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    private Guid GetSalePaymentStatusId(List<LookupDetail> lookupDetails, SalePaymentStatus paymentStatus)
    {
        return lookupDetails.FirstOrDefault(x => x.DevCode == (int)paymentStatus).Id;
    }
}
