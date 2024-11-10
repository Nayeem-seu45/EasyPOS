using EasyPOS.Application.Features.SaleReturns.Models;

namespace EasyPOS.Application.Features.SaleReturns.Queries;

[Authorize(Policy = Permissions.SaleReturns.View)]
public record GetSaleReturnListQuery
     : DataGridModel, ICacheableQuery<PaginatedResponse<SaleReturnModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.SaleReturn}_{PageNumber}_{PageSize}";
}

internal sealed class GetSaleReturnQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetSaleReturnListQuery, PaginatedResponse<SaleReturnModel>>
{
    public async Task<Result<PaginatedResponse<SaleReturnModel>>> Handle(GetSaleReturnListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(SaleReturnModel.Id)},
                t.ReturnDate As {nameof(SaleReturnModel.ReturnDate)},
                t.ReferenceNo AS {nameof(SaleReturnModel.ReferenceNo)},
                t.SoldReferenceNo AS {nameof(SaleReturnModel.SoldReferenceNo)},
                t.WarehouseId AS {nameof(SaleReturnModel.WarehouseId)},
                t.CustomerId AS {nameof(SaleReturnModel.CustomerId)},
                t.ReturnStatusId AS {nameof(SaleReturnModel.ReturnStatusId)},
                t.PaymentStatusId AS {nameof(SaleReturnModel.PaymentStatusId)},
                t.GrandTotal AS {nameof(SaleReturnModel.GrandTotal)},
                t.TotalPaidAmount AS {nameof(SaleReturnModel.PaidAmount)},
                t.DueAmount AS {nameof(SaleReturnModel.DueAmount)},
                t.ReturnNote AS {nameof(SaleReturnModel.ReturnNote)},
                t.StaffNote AS {nameof(SaleReturnModel.StaffNote)},
                w.Name AS {nameof(SaleReturnModel.WarehouseName)},
                c.Name AS {nameof(SaleReturnModel.CustomerName)},
                ss.Name AS {nameof(SaleReturnModel.ReturnStatus)},
                ps.Name AS {nameof(SaleReturnModel.PaymentStatus)}
            FROM dbo.SaleReturns AS t
            LEFT JOIN dbo.Warehouses w ON w.Id = t.WarehouseId
            LEFT JOIN dbo.Customers c ON c.Id = t.CustomerId
            LEFT JOIN dbo.LookupDetails ss ON ss.Id = t.ReturnStatusId
            LEFT JOIN dbo.LookupDetails ps ON ps.Id = t.PaymentStatusId
            WHERE 1 = 1
            """;


        return await PaginatedResponse<SaleReturnModel>
            .CreateAsync(connection, sql, request);

    }
}


