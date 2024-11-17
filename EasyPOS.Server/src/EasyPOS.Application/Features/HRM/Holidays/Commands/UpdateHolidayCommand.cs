namespace EasyPOS.Application.Features.HRM.Holidays.Commands;

public record UpdateHolidayCommand(
    Guid Id,
    string? Title,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Description, 
    bool IsActive
    ): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Holiday;
}

internal sealed class UpdateHolidayCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateHolidayCommand>
{
    public async Task<Result> Handle(UpdateHolidayCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Holidays.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
