﻿using EasyPOS.Domain.Stakeholders;
using Mapster;

namespace EasyPOS.Application.Features.Suppliers.Commands;

public record CreateSupplierCommand(
    string Name,
    string? Email,
    string? PhoneNo,
    string? Mobile,
    string? Country,
    string? City,
    string? Address,
    decimal OpeningBalance,
    bool IsActive) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Supplier;
}

internal sealed class CreateSupplierCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<CreateSupplierCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<Supplier>();

        dbContext.Suppliers.Add(entity);
        entity.OutstandingBalance = entity.CalculateOutstandingBalance();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}
