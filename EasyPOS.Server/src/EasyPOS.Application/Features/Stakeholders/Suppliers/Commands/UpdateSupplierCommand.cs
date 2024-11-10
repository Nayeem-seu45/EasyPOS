﻿namespace EasyPOS.Application.Features.Suppliers.Commands;

public record UpdateSupplierCommand(
    Guid Id,
    string Name,
    string? Email,
    string? PhoneNo,
    string? Mobile,
    string? Country,
    string? City,
    string? Address,
    bool IsActive) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Supplier;
}

internal sealed class UpdateSupplierCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<UpdateSupplierCommand>
{
    public async Task<Result> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Suppliers.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
