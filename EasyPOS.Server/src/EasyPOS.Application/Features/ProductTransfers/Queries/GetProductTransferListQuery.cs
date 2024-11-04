namespace EasyPOS.Application.Features.ProductTransfers.Queries;

[Authorize(Policy = Permissions.ProductTransfers.View)]
public record GetProductTransferListQuery
    : DataGridModel, ICacheableQuery<PaginatedResponse<ProductTransferModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.ProductTransfer}l_{PageNumber}_{PageSize}";
}

internal sealed class GetProductTransferListQueryHandler(ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetProductTransferListQuery, PaginatedResponse<ProductTransferModel>>
{
    public async Task<Result<PaginatedResponse<ProductTransferModel>>> Handle(GetProductTransferListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(ProductTransferModel.Id)},
                t.TransferDate AS {nameof(ProductTransferModel.TransferDate)},
                t.ReferenceNo AS {nameof(ProductTransferModel.ReferenceNo)},
                t.GrandTotal AS {nameof(ProductTransferModel.GrandTotal)},
                ts.Name AS {nameof(ProductTransferModel.TransferStatus)},
                fw.Name AS {nameof(ProductTransferModel.FromWarehouse)},
                tw.Name AS {nameof(ProductTransferModel.ToWarehouse)}
            FROM dbo.ProductTransfers t
            LEFT JOIN dbo.LookupDetails ts ON ts.Id = t.TransferStatusId
            LEFT JOIN dbo.Warehouses fw ON fw.Id = t.FromWarehouseId
            LEFT JOIN dbo.Warehouses tw ON tw.Id = t.ToWarehouseId
            """;

        var sqlWithOrders = $"""
                {sql} 
                ORDER BY t.TransferDate Desc
                """;

        return await PaginatedResponse<ProductTransferModel>
            .CreateAsync(connection, sql, request);
    }
}
