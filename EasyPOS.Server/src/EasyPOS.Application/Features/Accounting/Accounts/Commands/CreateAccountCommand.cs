using EasyPOS.Domain.Accounting;

namespace EasyPOS.Application.Features.Accounting.Accounts.Commands;

public record CreateAccountCommand(
    int AccountNo, 
    string Name, 
    decimal Balance, 
    string? Note
    ): ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Account;
}
    
internal sealed class CreateAccountCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateAccountCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
       var entity = request.Adapt<Account>();

       dbContext.Accounts.Add(entity);

       await dbContext.SaveChangesAsync(cancellationToken);

       return  entity.Id;
    }
}
