﻿using EasyPOS.Application.Features.Purchases.Shared;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Models;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;
using EasyPOS.Domain.Purchases;

namespace EasyPOS.Application.Features.Purchases.PurchasePayments.Commands;

public record CreatePurchasePaymentCommand(
    Guid PurchaseId,
    decimal ReceivedAmount,
    decimal PayingAmount,
    decimal ChangeAmount,
    Guid PaymentType,
    string? Note
    ) : ICacheInvalidatorCommand<Guid>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Purchase}";
}

internal sealed class CreatePurchasePaymentCommandHandler(
    IApplicationDbContext dbContext,
    ISupplierFinancialService supplierFinancialService,
    ICommonQueryService commonQueryService) : ICommandHandler<CreatePurchasePaymentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePurchasePaymentCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<PurchasePayment>();
        entity.PaymentDate = DateTime.Now;

        dbContext.PurchasePayments.Add(entity);

        var purchase = await dbContext.Purchases
            .FirstOrDefaultAsync(x => x.Id == entity.PurchaseId, cancellationToken: cancellationToken);

        if (purchase is null)
        {
            return Result.Failure<Guid>(Error.Failure(nameof(purchase), "Purchase Entity not found"));
        }

        purchase.PaidAmount += entity.PayingAmount;
        purchase.DueAmount = purchase.GrandTotal - purchase.PaidAmount;
        purchase.PaymentStatusId = await PurchaseSharedService.GetPurchasePaymentId(commonQueryService, purchase);

        // Adjust supplier financials for the new payment
        await supplierFinancialService.AdjustSupplierBalance(
            purchase.SupplierId, 
            entity.PayingAmount, 
            FinancialTransactionType.Payment,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

}

