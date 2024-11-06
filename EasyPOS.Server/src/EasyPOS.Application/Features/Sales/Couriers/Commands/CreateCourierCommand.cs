using EasyPOS.Domain.Sales;

namespace EasyPOS.Application.Features.Sales.Couriers.Commands;

public record CreateCourierCommand(
    string Name,
    string? PhoneNo,
    string? MobileNo,
    string? Email,
    string? Address
    ) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Courier;
}

internal sealed class CreateCourierCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<CreateCourierCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCourierCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<Courier>();

        dbContext.Couriers.Add(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
