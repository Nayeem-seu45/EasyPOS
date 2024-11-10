
using EasyPOS.Application.Features.SaleReturns.Models;

namespace EasyPOS.Application.Features.SaleReturns.Queries;

public record GetSaleReturnDetailByIdQuery(Guid Id) : ICacheableQuery<SaleReturnInfoModel>
{
    public string CacheKey => $"{CacheKeys.SaleReturn}_{Id}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => false;
}

internal sealed class GetSaleReturnDetailByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    ICommonQueryService commonQueryService)
    : IQueryHandler<GetSaleReturnDetailByIdQuery, SaleReturnInfoModel>
{
    public async Task<Result<SaleReturnInfoModel>> Handle(GetSaleReturnDetailByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return Result.Failure<SaleReturnInfoModel>(Error.Failure(nameof(SaleReturnInfoModel), ErrorMessages.NotFound));
        }

        var connection = sqlConnectionFactory.GetOpenConnection();

        // SQL query to get both SaleReturn and SaleReturnDetails with necessary fields
        var sql = $"""
            SELECT 
                t.Id AS {nameof(SaleReturnInfoModel.Id)},
                t.ReturnDate AS {nameof(SaleReturnInfoModel.ReturnDate)},
                t.ReferenceNo AS {nameof(SaleReturnInfoModel.ReferenceNo)},
                t.WarehouseId AS {nameof(SaleReturnInfoModel.WarehouseId)},
                t.CustomerId AS {nameof(SaleReturnInfoModel.CustomerId)},
                t.ReturnStatusId AS {nameof(SaleReturnInfoModel.ReturnStatusId)},
                t.AttachmentUrl AS {nameof(SaleReturnInfoModel.AttachmentUrl)},
                t.SubTotal AS {nameof(SaleReturnInfoModel.SubTotal)},
                t.TaxRate AS {nameof(SaleReturnInfoModel.TaxRate)},
                t.TaxAmount AS {nameof(SaleReturnInfoModel.TaxAmount)},
                t.DiscountType AS {nameof(SaleReturnInfoModel.DiscountType)},
                t.DiscountRate AS {nameof(SaleReturnInfoModel.DiscountRate)},
                t.DiscountAmount AS {nameof(SaleReturnInfoModel.DiscountAmount)},
                t.ShippingCost AS {nameof(SaleReturnInfoModel.ShippingCost)},
                t.GrandTotal AS {nameof(SaleReturnInfoModel.GrandTotal)},
                t.ReturnNote AS {nameof(SaleReturnInfoModel.ReturnNote)},

                -- SaleReturnDetails
                pd.Id AS {nameof(SaleReturnDetailModel.Id)},
                pd.SaleReturnId AS {nameof(SaleReturnDetailModel.SaleReturnId)},
                pd.ProductId AS {nameof(SaleReturnDetailModel.ProductId)},
                pd.ProductCode AS {nameof(SaleReturnDetailModel.ProductCode)},
                pd.ProductName AS {nameof(SaleReturnDetailModel.ProductName)},
                pd.ProductUnitCost AS {nameof(SaleReturnDetailModel.ProductUnitCost)},
                pd.ProductUnitPrice AS {nameof(SaleReturnDetailModel.ProductUnitPrice)},
                pd.ProductUnitId AS {nameof(SaleReturnDetailModel.ProductUnitId)},
                pd.ProductUnitDiscount AS {nameof(SaleReturnDetailModel.ProductUnitDiscount)},
                pd.SoldQuantity AS {nameof(SaleReturnDetailModel.SoldQuantity)},
                pd.ReturnedQuantity AS {nameof(SaleReturnDetailModel.ReturnedQuantity)},
                pd.BatchNo AS {nameof(SaleReturnDetailModel.BatchNo)},
                pd.ExpiredDate AS {nameof(SaleReturnDetailModel.ExpiredDate)},
                pd.NetUnitPrice AS {nameof(SaleReturnDetailModel.NetUnitPrice)},
                pd.DiscountType AS {nameof(SaleReturnDetailModel.DiscountType)},
                pd.DiscountRate AS {nameof(SaleReturnDetailModel.DiscountRate)},
                pd.DiscountAmount AS {nameof(SaleReturnDetailModel.DiscountAmount)},
                pd.TaxRate AS {nameof(SaleReturnDetailModel.TaxRate)},
                pd.TaxAmount AS {nameof(SaleReturnDetailModel.TaxAmount)},
                pd.TaxMethod AS {nameof(SaleReturnDetailModel.TaxMethod)},
                pd.TotalPrice AS {nameof(SaleReturnDetailModel.TotalPrice)}

            FROM dbo.SaleReturns t
            LEFT JOIN dbo.SaleReturnDetails pd ON pd.SaleReturnId = t.Id
            LEFT JOIN dbo.SaleReturnPayments pp ON pp.SaleReturnId = t.Id
            LEFT JOIN dbo.LookupDetails pt ON pt.Id = pp.PaymentType
            WHERE t.Id = @Id
            """;

        var saleDictionary = new Dictionary<Guid, SaleReturnInfoModel>();
        var saleDetailDictionary = new Dictionary<Guid, SaleReturnDetailModel>(); // Track details

        var result = await connection.QueryAsync<SaleReturnInfoModel, SaleReturnDetailModel, SaleReturnInfoModel>(
            sql,
            (sale, detail) =>
            {
                if (!saleDictionary.TryGetValue(sale.Id, out var saleEntry))
                {
                    saleEntry = sale;
                    saleEntry.SaleReturnDetails = [];
                    saleDictionary.Add(sale.Id, saleEntry);
                }

                // Add distinct SaleReturnDetails
                if (detail != null && !saleDetailDictionary.ContainsKey(detail.Id))
                {
                    saleEntry.SaleReturnDetails.Add(detail);
                    saleDetailDictionary[detail.Id] = detail; // Keep track of added details
                }

                return saleEntry;
            },
            new { request.Id },
            splitOn: "Id, Id, Id"
        );

        var sale = saleDictionary.Values.FirstOrDefault();
        if (sale is not null)
        {
            sale.TotalQuantity = sale.SaleReturnDetails.Count;
            sale.TotalDiscount = sale.SaleReturnDetails.Sum(x => x.DiscountAmount);
            sale.TotalTaxAmount = sale.SaleReturnDetails.Sum(x => x.TaxAmount);
            sale.TotalItems = $"{sale.TotalQuantity} ({sale.SaleReturnDetails.Sum(x => x.SoldQuantity)})";
            sale.CompanyInfo = await commonQueryService.GetCompanyInfoAsync(cancellationToken);
            sale.Customer = await commonQueryService.GetCustomerDetail(sale.CustomerId, cancellationToken);
            return sale;
        }
        else
        {
            return Result.Failure<SaleReturnInfoModel>(Error.Failure(nameof(SaleReturnInfoModel), ErrorMessages.NotFound));
        }
    }
}

