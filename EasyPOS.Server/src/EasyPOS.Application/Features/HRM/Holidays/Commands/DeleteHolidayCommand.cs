namespace EasyPOS.Application.Features.HRM.Holidays.Commands;

public record DeleteHolidayCommand(Guid Id): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Holiday;
}

internal sealed class DeleteHolidayCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteHolidayCommand>

{
    public async Task<Result> Handle(DeleteHolidayCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Holidays
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.Holidays.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}