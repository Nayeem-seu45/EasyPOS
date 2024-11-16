using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Application.Features.Sales.Models;
using EasyPOS.Application.Features.Stakeholders.Customers.Services;
using EasyPOS.Domain.Sales;
using static EasyPOS.Application.Common.Security.Permissions;

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
    ISaleService saleService,
    ICustomerService customerService)
    : ICommandHandler<CreateSaleCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = request.Adapt<Sale>();
        dbContext.Sales.Add(sale);

        sale.ReferenceNo = "S-" + DateTime.Now.ToString("yyyyMMddhhmmffff");

        await saleService
            .AdjustSaleAsync(SaleTransactionType.SaleCreate, sale, request.SalePayment?.PayingAmount ?? 0, cancellationToken);

        // Payment
        if (request.HasPayment
            && request.SalePayment is not null
            && request.SalePayment?.PayingAmount > 0
            && !request.SalePayment.PaymentType.IsNullOrEmpty())
        {
             saleService.AddPaymentToSale(
                 sale, 
                 request.SalePayment.ReceivedAmount, 
                 request.SalePayment.PayingAmount, 
                 request.SalePayment.ChangeAmount, 
                 request.SalePayment.PaymentType.Value, 
                 request.SalePayment.Note);
        }

        #region Customer
        // Customer
        var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.Id == sale.CustomerId);

        if (customer is null)
        {
            return Result.Failure<Guid>(Error.Failure(nameof(customer), "Customer not found."));
        }

        customerService.AdjustCustomerOnSale(SaleTransactionType.SaleCreate, customer, sale.DueAmount, sale.PaidAmount);

        #endregion

        await dbContext.SaveChangesAsync(cancellationToken);

        return sale.Id;
    }
}
