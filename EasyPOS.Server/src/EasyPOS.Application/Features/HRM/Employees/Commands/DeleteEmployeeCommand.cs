namespace EasyPOS.Application.Features.HRM.Employees.Commands;

public record DeleteEmployeeCommand(Guid Id): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Employee;
}

internal sealed class DeleteEmployeeCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteEmployeeCommand>

{
    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Employees
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.Employees.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}