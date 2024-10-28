namespace EasyPOS.Application.Features.Accounting.Expenses.Commands;

public record UpdateExpenseCommand(
    Guid Id,
    DateTime ExpenseDate, 
    string? Title, 
    string? ReferenceNo, 
    Guid WarehouseId, 
    Guid CategoryId, 
    decimal Amount, 
    Guid AccountId, 
    string? Description, 
    string? AttachmentUrl
    ): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Expense;
}

internal sealed class UpdateExpenseCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateExpenseCommand>
{
    public async Task<Result> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Expenses.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}