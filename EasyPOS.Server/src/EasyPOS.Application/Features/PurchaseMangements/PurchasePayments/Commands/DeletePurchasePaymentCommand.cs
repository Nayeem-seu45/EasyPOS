using EasyPOS.Application.Features.PurchaseMangements.Services;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Models;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;

namespace EasyPOS.Application.Features.Purchases.PurchasePayments.Commands;

public record DeletePurchasePaymentCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.PurchasePayment;
}

internal sealed class DeletePurchasePaymentCommandHandler(
    IApplicationDbContext dbContext,
    ISupplierFinancialService supplierFinancialService,
    IPurchaseService purchaseService)
    : ICommandHandler<DeletePurchasePaymentCommand>

{
    public async Task<Result> Handle(DeletePurchasePaymentCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.PurchasePayments
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        var purchase = await dbContext.Purchases
            .FirstOrDefaultAsync(x => x.Id == entity.PurchaseId, cancellationToken: cancellationToken);

        if (purchase is null) return Result.Failure(Error.NotFound(nameof(purchase), "Purchase Not Found."));

        dbContext.PurchasePayments.Remove(entity);

        purchase.PaidAmount -= entity.PayingAmount;
        purchase.DueAmount = purchase.GrandTotal - purchase.PaidAmount;

        await purchaseService.UpdatePurchasePaymentFieldsAsync(
            purchase, 
            entity.PayingAmount, 
            PurchaseTransactionType.PaymentDelete, 
            cancellationToken);

        // Adjust supplier balance for payment deletion
        await supplierFinancialService.AdjustSupplierBalance(
            purchase.SupplierId, 
            entity.PayingAmount, 
            PurchaseTransactionType.PaymentDelete,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
