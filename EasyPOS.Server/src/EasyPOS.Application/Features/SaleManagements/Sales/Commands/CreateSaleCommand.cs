using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Application.Features.Sales.Models;
using EasyPOS.Application.Features.Stakeholders.Customers.Services;
using EasyPOS.Application.Features.StockManagement.Services;
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
    ISaleService saleService,
    ICustomerService customerService,
    IStockService stockService)
    : ICommandHandler<CreateSaleCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        // Map request to Sale entity
        var sale = request.Adapt<Sale>();

        dbContext.Sales.Add(sale);

        // Generate a reference number
        sale.ReferenceNo = "S-" + DateTime.Now.ToString("yyyyMMddhhmmffff");

        // Validate and adjust stock for each sale detail
        foreach (var detail in request.SaleDetails)
        {
            var stockAdjustmentResult = await stockService.AdjustStockOnSaleAsync(
                detail.ProductId,
                sale.WarehouseId,
                detail.Quantity,
                isAddition: false // This is a sale, so reduce stock
            );

            if (!stockAdjustmentResult.IsSuccess)
            {
                return Result.Failure<Guid>(stockAdjustmentResult.Error); // Stop if stock adjustment fails
            }
        }

        // Adjust sale information using SaleService
        await saleService.AdjustSaleAsync(
            SaleTransactionType.SaleCreate,
            sale,
            request.SalePayment?.PayingAmount ?? 0,
            cancellationToken
        );

        // Handle payment if applicable
        if (request.HasPayment
            && request.SalePayment is not null
            && request.SalePayment.PayingAmount > 0
            && !request.SalePayment.PaymentType.IsNullOrEmpty())
        {
            saleService.AddPaymentToSale(
                sale,
                request.SalePayment.ReceivedAmount,
                request.SalePayment.PayingAmount,
                request.SalePayment.ChangeAmount,
                request.SalePayment.PaymentType.Value,
                request.SalePayment.Note
            );
        }

        #region Customer Adjustment

        // Retrieve customer
        var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.Id == sale.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure<Guid>(Error.Failure(nameof(customer), "Customer not found."));
        }

        // Adjust customer financials
        customerService.AdjustCustomerOnSale(
            SaleTransactionType.SaleCreate,
            customer,
            sale.DueAmount,
            sale.PaidAmount
        );

        #endregion

        // Save all changes
        await dbContext.SaveChangesAsync(cancellationToken);

        return sale.Id;
    }
}

