namespace EasyPOS.Application.Features.Accounting.Expenses.Commands;

public record DeleteExpenseCommand(Guid Id): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Expense;
}

internal sealed class DeleteExpenseCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteExpenseCommand>

{
    public async Task<Result> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Expenses
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.Expenses.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}