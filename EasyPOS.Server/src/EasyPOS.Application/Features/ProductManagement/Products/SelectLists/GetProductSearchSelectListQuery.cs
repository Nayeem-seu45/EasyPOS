using EasyPOS.Application.Features.ProductManagement.Products.SelectLists;

namespace EasyPOS.Application.Features.ProductManagement.Queries;

public record GetProductSearchSelectListQuery(
    string Query,
    bool? AllowCacheList) : ICacheableQuery<List<ProductSelectListModel>>
{
    public TimeSpan? Expiration => null;
    [JsonIgnore]
    public string CacheKey => CacheKeys.Product_All_SelectList;
    public bool? AllowCache => false;

}

internal sealed class GetProductSearchSelectListQueryHandler(
    ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetProductSearchSelectListQuery, List<ProductSelectListModel>>
{
    public async Task<Result<List<ProductSelectListModel>>> Handle(GetProductSearchSelectListQuery request, CancellationToken cancellationToken)
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
                p.DiscountAmount AS {nameof(ProductSelectListModel.DiscountAmount)}
            FROM dbo.Products p
            WHERE 
                (LOWER(p.Name) LIKE '%' + LOWER(@Query) + '%' 
                 OR LOWER(p.Code) LIKE '%' + LOWER(@Query) + '%')
            ORDER BY 
                CASE 
                    WHEN LOWER(p.Code) = LOWER(@Query) THEN 0
                    WHEN LOWER(p.Name) = LOWER(@Query) THEN 1
                    WHEN p.Code LIKE @Query + '%' THEN 2
                    WHEN p.Name LIKE @Query + '%' THEN 3
                    ELSE 4
                END,
                p.Name;
            """;

        var selectList = await connection.QueryAsync<ProductSelectListModel>(
            sql,
            new { request.Query }
        );

        return Result.Success(selectList.AsList());
    }
}




