using EasyPOS.Application.Features.PurchaseReturns.Models;
using EasyPOS.Application.Features.Purchases.Models;

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
                t.PurchaseReferenceNo AS {nameof(PurchaseReturnModel.PurchaseReferenceNo)},
                t.GrandTotal AS {nameof(PurchaseReturnModel.GrandTotal)},
                t.DueAmount AS {nameof(PurchaseReturnModel.DueAmount)},
                t.PaidAmount AS {nameof(PurchaseReturnModel.PaidAmount)},
                s.Name AS {nameof(PurchaseReturnModel.SupplierName)},
                pmns.Name AS {nameof(PurchaseModel.PaymentStatus)},
                [dbo].[fn_PaymentStatusTag](pmns.Name) AS {nameof(PurchaseModel.PaymentStatusTag)}         
            FROM dbo.PurchaseReturns t
            LEFT JOIN dbo.Suppliers s ON s.Id = t.SupplierId
            --LEFT JOIN dbo.LookupDetails ps ON ps.Id = t.ReturnStatusId
            LEFT JOIN dbo.LookupDetails pmns ON pmns.Id = t.PaymentStatusId           
            """;

        var sqlWithOrders = $"""
                {sql} 
                ORDER BY t.PurchaseReturnDate Desc
                """;

        return await PaginatedResponse<PurchaseReturnModel>
            .CreateAsync(connection, sql, request);
    }
}
