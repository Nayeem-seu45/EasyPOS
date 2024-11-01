using EasyPOS.Domain.Products;

namespace EasyPOS.Application.Features.ProductManagement.ProductAdjustments.Queries;

public record GetProductAdjustmentByIdQuery(Guid Id) : ICacheableQuery<ProductAdjustmentModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.ProductAdjustment}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetProductAdjustmentByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetProductAdjustmentByIdQuery, ProductAdjustmentModel>
{

    public async Task<Result<ProductAdjustmentModel>> Handle(GetProductAdjustmentByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new ProductAdjustmentModel();
        }
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(ProductAdjustmentModel.Id)},
                t.ReferenceNo AS {nameof(ProductAdjustmentModel.ReferenceNo)},
                t.WarehouseId AS {nameof(ProductAdjustmentModel.WarehouseId)},
                t.AttachmentUrl AS {nameof(ProductAdjustmentModel.AttachmentUrl)},
                t.Note AS {nameof(ProductAdjustmentModel.Note)},
                t.AdjDate AS {nameof(ProductAdjustmentModel.AdjDate)},
                t.TotalQuantity AS {nameof(ProductAdjustmentModel.TotalQuantity)},

                pad.Id {nameof(ProductAdjustmentDetailModel.Id)},
                pad.ProductAdjustmentId {nameof(ProductAdjustmentDetailModel.ProductAdjustmentId)},
                pad.ProductId {nameof(ProductAdjustmentDetailModel.ProductId)},
                pad.ProductName {nameof(ProductAdjustmentDetailModel.ProductName)},
                pad.ProductCode {nameof(ProductAdjustmentDetailModel.ProductCode)},
                pad.UnitCost {nameof(ProductAdjustmentDetailModel.UnitCost)},
                pad.Stock {nameof(ProductAdjustmentDetailModel.Stock)},
                pad.Quantity {nameof(ProductAdjustmentDetailModel.Quantity)},
                pad.ActionType {nameof(ProductAdjustmentDetailModel.ActionType)}
            FROM dbo.ProductAdjustments AS t
            LEFT JOIN dbo.ProductAdjustmentDetails pad ON pad.ProductAdjustmentId = t.Id
            WHERE t.Id = @Id
            """;

        var productAdjustments = new Dictionary<Guid, ProductAdjustmentModel>();

        var result = await connection.QueryAsync<ProductAdjustmentModel, ProductAdjustmentDetailModel, ProductAdjustmentModel>(
            sql,
            (productAdjustment, detail) =>
            {
                if (!productAdjustments.TryGetValue(productAdjustment.Id, out var productAdjustmentEntry))
                {
                    productAdjustmentEntry = productAdjustment;
                    productAdjustmentEntry.ProductAdjustmentDetails = [];
                    productAdjustments.Add(productAdjustmentEntry.Id, productAdjustmentEntry);
                }

                productAdjustmentEntry.ProductAdjustmentDetails.Add(detail);
                return productAdjustmentEntry;
            },
            new { request.Id },
            splitOn: "Id");
    
            var productAdjustment = productAdjustments.Values.FirstOrDefault();

        if(productAdjustment is null)
        {
            return Result.Failure<ProductAdjustmentModel>(Error.Failure(nameof(ProductAdjustment), "Product Adjustment not found."));
        }

        return productAdjustment;
    }
}

