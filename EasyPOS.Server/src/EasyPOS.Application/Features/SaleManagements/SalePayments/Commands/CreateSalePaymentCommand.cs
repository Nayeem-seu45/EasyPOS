using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Application.Features.Stakeholders.Customers.Services;
using EasyPOS.Domain.Sales;

namespace EasyPOS.Application.Features.Sales.SalePayments.Commands;

public record CreateSalePaymentCommand(
    Guid SaleId,
    decimal ReceivedAmount,
    decimal PayingAmount,
    decimal ChangeAmount,
    Guid PaymentType,
    string? Note
    ) : ICacheInvalidatorCommand<Guid>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Sale}";
}

internal sealed class CreateSalePaymentCommandHandler(
    IApplicationDbContext dbContext,
    ISaleService saleService,
    ICustomerService customerService) : ICommandHandler<CreateSalePaymentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSalePaymentCommand request, CancellationToken cancellationToken)
    {
        // Adapt the request to a SalePayment entity and set the payment date
        var salePayment = request.Adapt<SalePayment>();
        salePayment.PaymentDate = DateTime.Now;

        // Add sale payment to the database context
        dbContext.SalePayments.Add(salePayment);

        // Retrieve the associated sale
        var sale = await dbContext.Sales
            .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

        if (sale is null)
        {
            return Result.Failure<Guid>(Error.Failure(nameof(sale), "Sale entity not found"));
        }

        // Adjust sale's paid amount, due amount, and payment status
        await saleService.AdjustSaleAsync(
            SaleTransactionType.PaymentCreate,
            sale,
            request.PayingAmount,
            cancellationToken);

        // Retrieve the associated customer
        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == sale.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure<Guid>(Error.Failure(nameof(customer), "Customer not found"));
        }

        // Adjust customer's financial records
        customerService.AdjustCustomerOnPayment(
            SaleTransactionType.PaymentCreate,
            customer,
            request.PayingAmount);

        // Save changes to the database
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(salePayment.Id);
    }
}

