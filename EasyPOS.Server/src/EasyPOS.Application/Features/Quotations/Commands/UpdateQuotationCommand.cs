using EasyPOS.Application.Features.Quotations.Queries;

namespace EasyPOS.Application.Features.Quotations.Commands;

public record UpdateQuotationCommand : UpsertQuotationModel, ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Quotation;
}

internal sealed class UpdateQuotationCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<UpdateQuotationCommand>
{
    public async Task<Result> Handle(UpdateQuotationCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Quotations.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
