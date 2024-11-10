using EasyPOS.Application.Features.Sales.Shared;
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
    ICommonQueryService commonQueryService) : ICommandHandler<CreateSalePaymentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSalePaymentCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<SalePayment>();
        entity.PaymentDate = DateTime.Now;

        dbContext.SalePayments.Add(entity);

        var sale = await dbContext.Sales
            .FirstOrDefaultAsync(x => x.Id == entity.SaleId, cancellationToken: cancellationToken);

        if (sale is null)
        {
            return Result.Failure<Guid>(Error.Failure(nameof(sale), "Sale Entity not found"));
        }


        sale.PaidAmount += entity.PayingAmount;
        sale.DueAmount = sale.GrandTotal - sale.PaidAmount;

        var paymentStatusId = await SaleSharedService.GetSalePaymentId(commonQueryService, sale);

        if (paymentStatusId is not null)
        {
            sale.PaymentStatusId = paymentStatusId.Value;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}

