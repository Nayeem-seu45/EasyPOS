using EasyPOS.Application.Features.PurchaseReturns.Models;

namespace EasyPOS.Application.Features.PurchaseReturns.Queries;

[Authorize(Policy = Permissions.PurchaseReturns.View)]
public record GetPurchaseReturnListQuery
    : DataGridModel, ICacheableQuery<PaginatedResponse<PurchaseReturnModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.PurchaseReturn}l_{PageNumber}_{PageSize}";
}

internal sealed class GetPurchaseReturnListQueryHandler(ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetPurchaseReturnListQuery, PaginatedResponse<PurchaseReturnModel>>
{
    public async Task<Result<PaginatedResponse<PurchaseReturnModel>>> Handle(GetPurchaseReturnListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(PurchaseReturnModel.Id)},
                t.ReturnDate AS {nameof(PurchaseReturnModel.ReturnDate)},
                t.ReferenceNo AS {nameof(PurchaseReturnModel.ReferenceNo)},
                t.GrandTotal AS {nameof(PurchaseReturnModel.GrandTotal)},
                s.Name AS {nameof(PurchaseReturnModel.SupplierName)}
            FROM dbo.PurchaseReturns t
            LEFT JOIN dbo.Suppliers s ON s.Id = t.SupplierId
            --LEFT JOIN dbo.LookupDetails ps ON ps.Id = t.PurchaseReturnStatusId
            """;

        var sqlWithOrders = $"""
                {sql} 
                ORDER BY t.PurchaseReturnDate Desc
                """;

        return await PaginatedResponse<PurchaseReturnModel>
            .CreateAsync(connection, sql, request);
    }
}
