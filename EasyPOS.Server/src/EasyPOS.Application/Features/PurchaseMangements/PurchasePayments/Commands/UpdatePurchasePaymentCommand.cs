﻿using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Models;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;

namespace EasyPOS.Application.Features.Purchases.PurchasePayments.Commands;

public record UpdatePurchasePaymentCommand(
    Guid Id,
    Guid PurchaseId,
    decimal ReceivedAmount,
    decimal PayingAmount,
    decimal ChangeAmount,
    Guid PaymentType,
    string? Note
    ) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchasePayment;
}

internal sealed class UpdatePurchasePaymentCommandHandler(
    IApplicationDbContext dbContext,
    ISupplierFinancialService supplierFinancialService,
    IPurchaseService purchaseService)
    : ICommandHandler<UpdatePurchasePaymentCommand>
{
    public async Task<Result> Handle(UpdatePurchasePaymentCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.PurchasePayments.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        var purchase = await dbContext.Purchases
            .FirstOrDefaultAsync(x => x.Id == entity.PurchaseId, cancellationToken: cancellationToken);

        if (purchase is null) return Result.Failure(Error.NotFound(nameof(purchase), "Purchase Not Found."));

        var previousPaymentAmount = entity.PayingAmount;

        request.Adapt(entity);

        var paymentDifference = entity.PayingAmount - previousPaymentAmount;

        //purchase.PaidAmount += paymentDifference;
        //purchase.DueAmount = purchase.GrandTotal - purchase.PaidAmount;
        //purchase.PaymentStatusId = await PurchaseSharedService.GetPurchasePaymentId(commonQueryService, purchase);

        // Update purchase payment fields
        await purchaseService.AdjustPurchaseAndPaymentStatusAsync(
            purchase,
            paymentDifference,
            PurchaseTransactionType.PaymentUpdate,
            cancellationToken);


        // Adjust supplier financials based on the payment difference
        await supplierFinancialService.AdjustSupplierBalance(
            purchase.SupplierId, 
            paymentDifference, 
            PurchaseTransactionType.PaymentUpdate, 
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
