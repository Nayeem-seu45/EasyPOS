namespace EasyPOS.Application.Features.Sales.Couriers.Commands;

public record UpdateCourierCommand(
    Guid Id,
    string Name,
    string? PhoneNo,
    string? MobileNo,
    string? Email,
    string? Address
    ) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Courier;
}

internal sealed class UpdateCourierCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<UpdateCourierCommand>
{
    public async Task<Result> Handle(UpdateCourierCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Couriers.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
