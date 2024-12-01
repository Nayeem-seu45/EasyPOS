using EasyPOS.Application.Features.Quotations.Models;
using EasyPOS.Domain.Quotations;

namespace EasyPOS.Application.Features.Quotations.Commands;

public record CreateQuotationCommand : UpsertQuotationModel, ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Quotation;
}

internal sealed class CreateQuotationCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<CreateQuotationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateQuotationCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<Quotation>();

        dbContext.Quotations.Add(entity);
        entity.ReferenceNo = UtilityExtensions.GetDateTimeStampRef("QT-");

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
