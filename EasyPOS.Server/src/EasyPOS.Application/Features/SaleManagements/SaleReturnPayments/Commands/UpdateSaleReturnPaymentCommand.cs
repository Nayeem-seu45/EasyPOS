using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Application.Features.Stakeholders.Customers.Services;

namespace EasyPOS.Application.Features.Sales.SaleReturnPayments.Commands;

public record UpdateSaleReturnPaymentCommand(
    Guid Id,
    Guid SaleReturnId,
    decimal ReceivedAmount,
    decimal PayingAmount,
    decimal ChangeAmount,
    Guid PaymentType,
    string? Note
    ) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.SaleReturnPayment;
}

internal sealed class UpdateSaleReturnPaymentCommandHandler(
    IApplicationDbContext dbContext,
    ISaleReturnService saleReturnService)
    : ICommandHandler<UpdateSaleReturnPaymentCommand>
{
    public async Task<Result> Handle(UpdateSaleReturnPaymentCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the existing sale payment
        var saleReturnPayment = await dbContext.SaleReturnPayments
            .FindAsync([request.Id], cancellationToken);

        if (saleReturnPayment is null)
            return Result.Failure(Error.NotFound(nameof(saleReturnPayment), ErrorMessages.EntityNotFound));

        // Retrieve the associated sale
        var saleReturn = await dbContext.SaleReturns
            .FirstOrDefaultAsync(x => x.Id == saleReturnPayment.SaleReturnId, cancellationToken);

        if (saleReturn is null)
            return Result.Failure(Error.NotFound(nameof(saleReturn), "SaleReturn not found."));

        // Calculate the previous payment amount
        var previousPaymentAmount = saleReturnPayment.PayingAmount;

        // Update payment entity with new values
        request.Adapt(saleReturnPayment);

        // Adjust sale based on payment update
        var paymentDifference = saleReturnPayment.PayingAmount - previousPaymentAmount;

        await saleReturnService.AdjustSaleReturnAsync(
            SaleReturnTransactionType.ReturnPaymentUpdate,
            saleReturn,
            paymentDifference,
            cancellationToken);

        // Save changes to the database
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
