using EasyPOS.Domain.Products;

namespace EasyPOS.Application.Features.ProductManagement.CountStocks.Commands;

public record CreateCountStockCommand(
    Guid WarehouseId,
    CountStockType Type,
    List<Guid> CategoryIds,
    List<Guid> BrandIds
    ) : ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.CountStock;
}

internal sealed class CreateCountStockCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<CreateCountStockCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCountStockCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<CountStock>();

        IEnumerable<CountStockCategory> csCategories = request.CategoryIds.Select(x => new CountStockCategory
        {
            CategoryId = x,
        }).ToList();

        IEnumerable<CountStockBrand> csBrands = request.CategoryIds.Select(x => new CountStockBrand
        {
            BrandId = x,
        }).ToList();

        entity.CountStockCategories.AddRange(csCategories);
        entity.CountStockBrands.AddRange(csBrands);

        dbContext.CountStocks.Add(entity);

        entity.CountingDate = DateTime.Now;
        entity.ReferenceNo = DateTime.Now.ToString("yyyyMMddHHmmssfff");

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
