using EasyPOS.Application.Features.ProductManagement.Products.SelectLists;

namespace EasyPOS.Application.Features.ProductManagement.Queries;

public record GetProductSearchInStockSelectListQuery(
    string Query,
    Guid WarehouseId,
    bool? AllowCacheList) : ICacheableQuery<List<ProductSelectListModel>>
{
    public TimeSpan? Expiration => null;
    [JsonIgnore]
    public string CacheKey => CacheKeys.Product_All_SelectList;
    public bool? AllowCache => false;

}

internal sealed class GetProductSearchInStockSelectListQueryHandler(
    ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetProductSearchInStockSelectListQuery, List<ProductSelectListModel>>
{
    public async Task<Result<List<ProductSelectListModel>>> Handle(GetProductSearchInStockSelectListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        string sql = $"""
            SELECT TOP 10
                p.Id AS {nameof(ProductSelectListModel.Id)},
                p.Name AS {nameof(ProductSelectListModel.Name)},
                p.Code AS {nameof(ProductSelectListModel.Code)},
                p.PurchaseUnit AS {nameof(ProductSelectListModel.PurchaseUnit)},
                p.SaleUnit AS {nameof(ProductSelectListModel.SaleUnit)},
                p.CostPrice AS {nameof(ProductSelectListModel.CostPrice)},
                p.SalePrice AS {nameof(ProductSelectListModel.SalePrice)},
                p.TaxRate AS {nameof(ProductSelectListModel.TaxRate)},
                p.TaxMethod AS {nameof(ProductSelectListModel.TaxMethod)},
                p.DiscountType AS {nameof(ProductSelectListModel.DiscountType)},
                p.DiscountRate AS {nameof(ProductSelectListModel.DiscountRate)},
                p.DiscountAmount AS {nameof(ProductSelectListModel.DiscountAmount)},
                s.Quantity AS {nameof(ProductSelectListModel.AvailableStock)}
            FROM dbo.Products p
            INNER JOIN dbo.Stocks s 
                ON p.Id = s.ProductId 
                AND s.WarehouseId = @WarehouseId
            WHERE 
                (p.Name LIKE '%' + @Query + '%' 
                 OR p.Code LIKE '%' + @Query + '%')
                --AND s.Quantity > 0
            ORDER BY 
                CASE 
                    WHEN p.Code LIKE @Query + '%' THEN 1
                    WHEN p.Name LIKE @Query + '%' THEN 2
                    ELSE 3
                END,
                p.Name;
            """;

        var selectList = await connection.QueryAsync<ProductSelectListModel>(
            sql,
            new { request.Query, request.WarehouseId }
        );

        return Result.Success(selectList.AsList());
    }
}




