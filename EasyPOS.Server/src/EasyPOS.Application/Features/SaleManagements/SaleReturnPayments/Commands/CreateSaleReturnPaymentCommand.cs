using EasyPOS.Application.Features.SaleManagements.Services;
using EasyPOS.Application.Features.SaleManagements.Shared;
using EasyPOS.Domain.Sales;

namespace EasyPOS.Application.Features.Sales.SaleReturnPayments.Commands;

public record CreateSaleReturnPaymentCommand(
    Guid SaleReturnId,
    decimal ReceivedAmount,
    decimal PayingAmount,
    decimal ChangeAmount,
    Guid PaymentType,
    string? Note
    ) : ICacheInvalidatorCommand<Guid>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.SaleReturn}";
}

internal sealed class CreateSaleReturnPaymentCommandHandler(
    IApplicationDbContext dbContext,
    ISaleReturnService saleReturnService) : ICommandHandler<CreateSaleReturnPaymentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSaleReturnPaymentCommand request, CancellationToken cancellationToken)
    {
        // Adapt the request to a SaleReturnPayment entity and set the payment date
        var saleReturnPayment = request.Adapt<SaleReturnPayment>();
        saleReturnPayment.PaymentDate = DateTime.Now;

        // Add sale payment to the database context
        dbContext.SaleReturnPayments.Add(saleReturnPayment);

        // Retrieve the associated sale
        var saleReturn = await dbContext.SaleReturns
            .FirstOrDefaultAsync(s => s.Id == request.SaleReturnId, cancellationToken);

        if (saleReturn is null)
        {
            return Result.Failure<Guid>(Error.Failure(nameof(saleReturn), "SaleReturn entity not found"));
        }

        // Adjust sale's paid amount, due amount, and payment status
        await saleReturnService.AdjustSaleReturnAsync(
            SaleReturnTransactionType.SaleReturnCreate,
            saleReturn,
            saleReturnPayment.PayingAmount,
            cancellationToken);

        // Save changes to the database
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(saleReturnPayment.Id);
    }
}

