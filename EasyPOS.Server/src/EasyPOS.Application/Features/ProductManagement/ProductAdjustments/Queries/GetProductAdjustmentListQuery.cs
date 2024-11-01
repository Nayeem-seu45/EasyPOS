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
                t.AttachmentUrl AS {nameof(ProductAdjustmentModel.AttachmentUrl)},
                t.Note AS {nameof(ProductAdjustmentModel.Note)},
                t.AdjDate AS {nameof(ProductAdjustmentModel.AdjDate)}
            FROM dbo.ProductAdjustments AS t
            WHERE 1 = 1
            """;

        return await PaginatedResponse<ProductAdjustmentModel>
            .CreateAsync(connection, sql, request);

    }
}


