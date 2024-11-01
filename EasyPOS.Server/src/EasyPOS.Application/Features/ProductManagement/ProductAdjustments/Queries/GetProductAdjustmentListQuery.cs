namespace EasyPOS.Application.Features.ProductManagement.ProductAdjustments.Queries;

[Authorize(Policy = Permissions.ProductAdjustments.View)]
public record GetProductAdjustmentListQuery 
     : DataGridModel, ICacheableQuery<PaginatedResponse<ProductAdjustmentModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.ProductAdjustment}_{PageNumber}_{PageSize}";
     
}

internal sealed class GetProductAdjustmentQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetProductAdjustmentListQuery, PaginatedResponse<ProductAdjustmentModel>>
{
    public async Task<Result<PaginatedResponse<ProductAdjustmentModel>>> Handle(GetProductAdjustmentListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(ProductAdjustmentModel.Id)},
                t.WarehouseId AS {nameof(ProductAdjustmentModel.WarehouseId)},
                t.ReferenceNo AS {nameof(ProductAdjustmentModel.ReferenceNo)},
                t.TotalQuantity AS {nameof(ProductAdjustmentModel.TotalQuantity)},
                --t.Note AS {nameof(ProductAdjustmentModel.Note)},
                t.AdjDate AS {nameof(ProductAdjustmentModel.AdjDate)},
                w.Name AS {nameof(ProductAdjustmentModel.Warehouse)}
            FROM dbo.ProductAdjustments AS t
            LEFT JOIN dbo.Warehouses AS w ON w.Id = t.WarehouseId
            WHERE 1 = 1
            """;

        return await PaginatedResponse<ProductAdjustmentModel>
            .CreateAsync(connection, sql, request);

    }
}


