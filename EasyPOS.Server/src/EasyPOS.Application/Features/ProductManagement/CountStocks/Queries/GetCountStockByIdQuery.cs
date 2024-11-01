namespace EasyPOS.Application.Features.ProductManagement.CountStocks.Queries;

public record GetCountStockByIdQuery(Guid Id) : ICacheableQuery<CountStockModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.CountStock}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetCountStockByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetCountStockByIdQuery, CountStockModel>
{

    public async Task<Result<CountStockModel>> Handle(GetCountStockByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new CountStockModel();
        }
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(CountStockModel.Id)},
                t.WarehouseId AS {nameof(CountStockModel.WarehouseId)},
                t.CountingDate AS {nameof(CountStockModel.CountingDate)}
            FROM dbo.CountStocks AS t
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<CountStockModel>(sql, new { request.Id });
    }
}

