namespace EasyPOS.Application.Features.HRM.WorkingShifts.Commands;

public record DeleteWorkingShiftCommand(Guid Id): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.WorkingShift;
}

internal sealed class DeleteWorkingShiftCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteWorkingShiftCommand>

{
    public async Task<Result> Handle(DeleteWorkingShiftCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.WorkingShifts
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.WorkingShifts.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}