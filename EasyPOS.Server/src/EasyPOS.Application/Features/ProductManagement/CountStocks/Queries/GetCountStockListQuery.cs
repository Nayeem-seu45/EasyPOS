namespace EasyPOS.Application.Features.ProductManagement.CountStocks.Queries;

[Authorize(Policy = Permissions.CountStocks.View)]
public record GetCountStockListQuery 
     : DataGridModel, ICacheableQuery<PaginatedResponse<CountStockModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.CountStock}_{PageNumber}_{PageSize}";
     
}

internal sealed class GetCountStockQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetCountStockListQuery, PaginatedResponse<CountStockModel>>
{
    public async Task<Result<PaginatedResponse<CountStockModel>>> Handle(GetCountStockListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(CountStockModel.Id)},
                t.CountingDate AS {nameof(CountStockModel.CountingDate)},
                t.Type AS {nameof(CountStockModel.Type)},
                t.ReferenceNo AS {nameof(CountStockModel.ReferenceNo)},
                IIF(t.Type = 1, 'Full', 'Partial') AS {nameof(CountStockModel.TypeName)},
                w.Name AS {nameof(CountStockModel.Warehouse)}
            FROM dbo.CountStocks AS t
            LEFT JOIN dbo.Warehouses AS w ON w.Id = t.WarehouseId
            WHERE 1 = 1
            """;


        return await PaginatedResponse<CountStockModel>
            .CreateAsync(connection, sql, request);

    }
}


