using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Application.Features.Stakeholders.Customers.Services;

namespace EasyPOS.Application.Features.Sales.SaleReturnPayments.Commands;

public record DeleteSaleReturnPaymentCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SaleReturnPayment;
}

internal sealed class DeleteSaleReturnPaymentCommandHandler(
    IApplicationDbContext dbContext,
    ISaleReturnService saleReturnService)
    : ICommandHandler<DeleteSaleReturnPaymentCommand>
{
    public async Task<Result> Handle(DeleteSaleReturnPaymentCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the existing sale payment
        var saleReturnPayment = await dbContext.SaleReturnPayments
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (saleReturnPayment is null)
            return Result.Failure(Error.NotFound(nameof(saleReturnPayment), ErrorMessages.EntityNotFound));

        // Retrieve the associated sale
        var saleReturn = await dbContext.SaleReturns
            .FirstOrDefaultAsync(x => x.Id == saleReturnPayment.SaleReturnId, cancellationToken);

        if (saleReturn is null)
            return Result.Failure(Error.NotFound(nameof(saleReturn), "SaleReturn not found."));

        // Remove the payment from the sale
        dbContext.SaleReturnPayments.Remove(saleReturnPayment);

        // Adjust the sale’s paid and due amounts after removing the payment
        await saleReturnService.AdjustSaleReturnAsync(
            SaleReturnTransactionType.ReturnPaymentDelete,
            saleReturn,
            saleReturnPayment.PayingAmount,
            cancellationToken);

        // Save changes to the database
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

