using EasyPOS.Application.Features.Stakeholders.Suppliers.Models;
using EasyPOS.Application.Features.Stakeholders.Suppliers.Services;

namespace EasyPOS.Application.Features.Purchases.Commands;

public record DeletePurchaseCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Purchase;
}

internal sealed class DeletePurchaseCommandHandler(
    IApplicationDbContext dbContext,
    ISupplierFinancialService supplierFinancialService)
    : ICommandHandler<DeletePurchaseCommand>
{
    public async Task<Result> Handle(DeletePurchaseCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Purchases.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.Purchases.Remove(entity);

        // Adjust supplier financials by reversing due amount
        await supplierFinancialService.AdjustSupplierBalance(
            entity.SupplierId,
            entity.DueAmount, 
            PurchaseTransactionType.PurchaseDelete, 
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
