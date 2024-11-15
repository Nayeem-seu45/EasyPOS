using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Application.Features.Stakeholders.Customers.Services;

namespace EasyPOS.Application.Features.Sales.SalePayments.Commands;

public record DeleteSalePaymentCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SalePayment;
}

internal sealed class DeleteSalePaymentCommandHandler(
    IApplicationDbContext dbContext,
    ISaleService saleService,
    ICustomerService customerService)
    : ICommandHandler<DeleteSalePaymentCommand>
{
    public async Task<Result> Handle(DeleteSalePaymentCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the existing sale payment
        var salePayment = await dbContext.SalePayments
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (salePayment is null)
            return Result.Failure(Error.NotFound(nameof(salePayment), ErrorMessages.EntityNotFound));

        // Retrieve the associated sale
        var sale = await dbContext.Sales
            .FirstOrDefaultAsync(x => x.Id == salePayment.SaleId, cancellationToken);

        if (sale is null)
            return Result.Failure(Error.NotFound(nameof(sale), "Sale not found."));

        // Remove the payment from the sale
        dbContext.SalePayments.Remove(salePayment);

        // Adjust the sale’s paid and due amounts after removing the payment
        await saleService.AdjustSaleAsync(
            SaleTransactionType.PaymentDelete,
            sale,
            -salePayment.PayingAmount,
            cancellationToken);

        // Retrieve the associated customer
        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == sale.CustomerId, cancellationToken);

        if (customer is null)
            return Result.Failure(Error.Failure(nameof(customer), "Customer not found."));

        // Adjust the customer’s total due and paid amounts after payment deletion
        customerService.AdjustCustomerOnPayment(
            SaleTransactionType.PaymentDelete,
            customer,
            salePayment.PayingAmount);

        // Save changes to the database
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

