namespace EasyPOS.Application.Features.Quotations.Commands;

public record DeleteQuotationDetailCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Quotation;
}

internal sealed class DeleteQuotationDetailCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<DeleteQuotationDetailCommand>
{
    public async Task<Result> Handle(DeleteQuotationDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.QuotationDetails.FindAsync(request.Id, cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.QuotationDetails.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
