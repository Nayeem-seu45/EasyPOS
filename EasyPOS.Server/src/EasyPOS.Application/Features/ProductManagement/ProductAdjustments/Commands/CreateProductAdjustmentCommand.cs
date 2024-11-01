using EasyPOS.Application.Features.ProductManagement.ProductAdjustments.Queries;
using EasyPOS.Domain.Products;

namespace EasyPOS.Application.Features.ProductManagement.ProductAdjustments.Commands;

public record CreateProductAdjustmentCommand(
    Guid WarehouseId, 
    string? AttachmentUrl, 
    string? Note, 
    DateTime AdjDate,
    List<ProductAdjustmentDetailModel> ProductAdjustmentDetails
    ): ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.ProductAdjustment;
}
    
internal sealed class CreateProductAdjustmentCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateProductAdjustmentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductAdjustmentCommand request, CancellationToken cancellationToken)
    {
       var entity = request.Adapt<ProductAdjustment>();

       dbContext.ProductAdjustments.Add(entity);

       entity.TotalQuantity = entity.ProductAdjustmentDetails.Count;

       await dbContext.SaveChangesAsync(cancellationToken);

       return  entity.Id;
    }
}
