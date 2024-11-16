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
                t.ReturnDate AS {nameof(SaleReturnModel.ReturnDate)},
                t.ReferenceNo AS {nameof(SaleReturnModel.ReferenceNo)},
                t.SoldReferenceNo AS {nameof(SaleReturnModel.SoldReferenceNo)},
                t.GrandTotal AS {nameof(SaleReturnModel.GrandTotal)},
                t.DueAmount AS {nameof(SaleReturnModel.DueAmount)},
                t.PaidAmount AS {nameof(SaleReturnModel.PaidAmount)},
                c.Name AS {nameof(SaleReturnModel.CustomerName)},
                pmns.Name AS {nameof(SaleReturnModel.PaymentStatus)},
                [dbo].[fn_PaymentStatusTag](pmns.Name) AS {nameof(SaleReturnModel.PaymentStatusTag)}         
            FROM dbo.SaleReturns AS t
            LEFT JOIN dbo.Warehouses w ON w.Id = t.WarehouseId
            LEFT JOIN dbo.Customers c ON c.Id = t.CustomerId
            --LEFT JOIN dbo.LookupDetails ss ON ss.Id = t.ReturnStatusId
            LEFT JOIN dbo.LookupDetails pmns ON pmns.Id = t.PaymentStatusId           
            WHERE 1 = 1
            """;


        return await PaginatedResponse<SaleReturnModel>
            .CreateAsync(connection, sql, request);

    }
}


