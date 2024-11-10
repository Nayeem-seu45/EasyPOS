using EasyPOS.Application.Features.SaleReturns.Models;

namespace EasyPOS.Application.Features.SaleReturns.Queries;

public record GetSaleReturnBySaleIdQuery(Guid SaleId) : ICacheableQuery<UpsertSaleReturnModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.SaleReturn}_{SaleId}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;
}

internal sealed class GetSaleReturnBySaleIdQueryHandler(ISqlConnectionFactory sqlConnection, ICommonQueryService commonQueryService)
    : IQueryHandler<GetSaleReturnBySaleIdQuery, UpsertSaleReturnModel>
{
    public async Task<Result<UpsertSaleReturnModel>> Handle(GetSaleReturnBySaleIdQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                -- UpsertSaleReturnModel fields (master)
                s.Id AS {nameof(UpsertSaleReturnModel.Id)},
                s.ReturnDate AS {nameof(UpsertSaleReturnModel.ReturnDate)},
                s.ReferenceNo AS {nameof(UpsertSaleReturnModel.SoldReferenceNo)},
                s.WarehouseId AS {nameof(UpsertSaleReturnModel.WarehouseId)},
                s.CustomerId AS {nameof(UpsertSaleReturnModel.CustomerId)},
                s.BillerId AS {nameof(UpsertSaleReturnModel.BillerId)},
                s.ReturnNote AS {nameof(UpsertSaleReturnModel.ReturnNote)},
                s.StaffNote AS {nameof(UpsertSaleReturnModel.StaffNote)},
                c.Name AS {nameof(UpsertSaleReturnModel.Customer)},

                -- SaleReturnDetailModel fields (detail)
                d.Id AS {nameof(SaleReturnDetailModel.Id)},
                d.ProductId AS {nameof(SaleReturnDetailModel.ProductId)},
                d.ProductCode AS {nameof(SaleReturnDetailModel.ProductCode)},
                d.ProductName AS {nameof(SaleReturnDetailModel.ProductName)},
                d.ProductUnitCost AS {nameof(SaleReturnDetailModel.ProductUnitCost)},
                d.ProductUnitPrice AS {nameof(SaleReturnDetailModel.ProductUnitPrice)},
                d.ProductUnitId AS {nameof(SaleReturnDetailModel.ProductUnitId)},
                d.ProductUnit AS {nameof(SaleReturnDetailModel.ProductUnit)},
                d.ProductUnitDiscount AS {nameof(SaleReturnDetailModel.ProductUnitDiscount)},
                d.Quantity AS {nameof(SaleReturnDetailModel.SoldQuantity)},
                d.BatchNo AS {nameof(SaleReturnDetailModel.BatchNo)},
                d.ExpiredDate AS {nameof(SaleReturnDetailModel.ExpiredDate)},
                d.NetUnitPrice AS {nameof(SaleReturnDetailModel.NetUnitPrice)},
                d.DiscountType AS {nameof(SaleReturnDetailModel.DiscountType)},
                d.DiscountRate AS {nameof(SaleReturnDetailModel.DiscountRate)},
                d.DiscountAmount AS {nameof(SaleReturnDetailModel.DiscountAmount)},
                d.TaxRate AS {nameof(SaleReturnDetailModel.TaxRate)},
                d.TaxAmount AS {nameof(SaleReturnDetailModel.TaxAmount)},
                d.TaxMethod AS {nameof(SaleReturnDetailModel.TaxMethod)},
                d.TotalPrice AS {nameof(SaleReturnDetailModel.TotalPrice)}
            FROM dbo.Sales s
            LEFT JOIN dbo.SaleDetails d ON s.Id = d.SaleId
            LEFT JOIN dbo.Customers c ON c.Id = s.CustomerId
            WHERE s.Id = @Id
        """;

        var saleDictionary = new Dictionary<Guid, UpsertSaleReturnModel>();

        var saleModel = await connection.QueryAsync<UpsertSaleReturnModel, SaleReturnDetailModel, UpsertSaleReturnModel>(
            sql,
            (sale, saleDetail) =>
            {
                if (!saleDictionary.TryGetValue(sale.Id, out var currentSaleReturn))
                {
                    currentSaleReturn = sale;
                    currentSaleReturn.SaleReturnDetails = [];
                    saleDictionary.Add(currentSaleReturn.Id, currentSaleReturn);
                }

                if (saleDetail != null)
                {
                    currentSaleReturn.SaleReturnDetails.Add(saleDetail);
                }

                return currentSaleReturn;
            },
            new {Id = request.SaleId },
            splitOn: nameof(SaleReturnDetailModel.Id)
        );

        var result = saleModel.FirstOrDefault();
        result.ReturnDate = DateOnly.FromDateTime(DateTime.Now);
        return result == null
            ? Result.Failure<UpsertSaleReturnModel>(Error.Failure(nameof(UpsertSaleReturnModel), ErrorMessages.NotFound))
            : Result.Success(result);
    }
}

