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
                t.ProductTransferDate AS {nameof(ProductTransferModel.TransferDate)},
                t.ReferenceNo AS {nameof(ProductTransferModel.ReferenceNo)},
                t.GrandTotal AS {nameof(ProductTransferModel.GrandTotal)},
                ps.Name AS {nameof(ProductTransferModel.TransferStatus)},
            FROM dbo.ProductTransfers t
            LEFT JOIN dbo.LookupDetails ps ON ps.Id = t.TransferStatusId
            """;

        var sqlWithOrders = $"""
                {sql} 
                ORDER BY t.ProductTransferDate Desc
                """;

        return await PaginatedResponse<ProductTransferModel>
            .CreateAsync(connection, sql, request);
    }
}
