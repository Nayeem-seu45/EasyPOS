namespace EasyPOS.Application.Features.Quotations.Commands;

public record DeleteQuotationCommand(Guid Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Quotation;
}

internal sealed class DeleteQuotationCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<DeleteQuotationCommand>

{
    public async Task<Result> Handle(DeleteQuotationCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Quotations
            .FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.Quotations.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}
