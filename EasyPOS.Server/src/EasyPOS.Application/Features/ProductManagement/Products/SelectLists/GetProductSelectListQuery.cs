namespace EasyPOS.Application.Features.ProductManagement.Products.SelectLists;

public record GetProductSelectListQuery(
    bool? AllowCacheList) : ICacheableQuery<List<ProductSelectListModel>>
{
    public TimeSpan? Expiration => null;
    [JsonIgnore]
    public string CacheKey => CacheKeys.Product_All_SelectList;
    public bool? AllowCache => AllowCacheList ?? true;

}

internal sealed class ProductSelectListQueryHandler(
    ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetProductSelectListQuery, List<ProductSelectListModel>>
{
    public async Task<Result<List<ProductSelectListModel>>> Handle(GetProductSelectListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        string sql = $"""
            SELECT 
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
                ISNULL(s.Quantity, 0) AS {nameof(ProductSelectListModel.AvailableStock)}
            FROM dbo.Products p
            LEFT JOIN dbo.Stocks s ON p.Id = s.ProductId
            --LEFT JOIN dbo.Stocks s ON p.Id = s.ProductId AND s.WarehouseId = @WarehouseId
            WHERE 1 = 1
            ORDER BY p.Name
            """;


        var selectList = await connection
                .QueryAsync<ProductSelectListModel>(sql);


        //var selectList = await connection
        //        .QueryAsync<ProductSelectListModel>(sql, new { request.WarehouseId });

        return Result.Success(selectList.AsList());
    }
}


