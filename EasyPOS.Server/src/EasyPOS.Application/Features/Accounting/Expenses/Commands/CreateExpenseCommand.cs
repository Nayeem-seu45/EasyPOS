using EasyPOS.Domain.Accounting;

namespace EasyPOS.Application.Features.Accounting.Expenses.Commands;

public record CreateExpenseCommand(
    DateTime ExpenseDate, 
    string? Title, 
    string? ReferenceNo, 
    Guid WarehouseId, 
    Guid CategoryId, 
    decimal Amount, 
    Guid AccountId, 
    string? Description, 
    string? AttachmentUrl
    ): ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Expense;
}
    
internal sealed class CreateExpenseCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateExpenseCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
       var entity = request.Adapt<Expense>();

       dbContext.Expenses.Add(entity);

       await dbContext.SaveChangesAsync(cancellationToken);

       return  entity.Id;
    }
}