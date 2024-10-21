using EasyPOS.Application.Features.Trades.SalePayments.Queries;

namespace EasyPOS.Application.Features.Trades.Sales.Queries;

public record GetSaleDetailByIdQuery(Guid Id) : ICacheableQuery<SaleInfoModel>
{
    public string CacheKey => $"{CacheKeys.Sale}_{Id}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => false;
}

internal sealed class GetSaleDetailByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    ICommonQueryService commonQueryService) 
    : IQueryHandler<GetSaleDetailByIdQuery, SaleInfoModel>
{
    public async Task<Result<SaleInfoModel>> Handle(GetSaleDetailByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return Result.Failure<SaleInfoModel>(Error.Failure(nameof(SaleInfoModel), ErrorMessages.NotFound));
        }

        var connection = sqlConnectionFactory.GetOpenConnection();

        // SQL query to get both Sale and SaleDetails with necessary fields
        var sql = $"""
            SELECT 
                t.Id AS {nameof(SaleInfoModel.Id)},
                t.SaleDate AS {nameof(SaleInfoModel.SaleDate)},
                t.ReferenceNo AS {nameof(SaleInfoModel.ReferenceNo)},
                t.WarehouseId AS {nameof(SaleInfoModel.WarehouseId)},
                t.CustomerId AS {nameof(SaleInfoModel.CustomerId)},
                t.SaleStatusId AS {nameof(SaleInfoModel.SaleStatusId)},
                t.AttachmentUrl AS {nameof(SaleInfoModel.AttachmentUrl)},
                t.SubTotal AS {nameof(SaleInfoModel.SubTotal)},
                t.TaxRate AS {nameof(SaleInfoModel.TaxRate)},
                t.TaxAmount AS {nameof(SaleInfoModel.TaxAmount)},
                t.DiscountType AS {nameof(SaleInfoModel.DiscountType)},
                t.DiscountRate AS {nameof(SaleInfoModel.DiscountRate)},
                t.DiscountAmount AS {nameof(SaleInfoModel.DiscountAmount)},
                t.ShippingCost AS {nameof(SaleInfoModel.ShippingCost)},
                t.GrandTotal AS {nameof(SaleInfoModel.GrandTotal)},
                t.SaleNote AS {nameof(SaleInfoModel.SaleNote)},

                -- SaleDetails
                pd.Id AS {nameof(SaleDetailModel.Id)},
                pd.SaleId AS {nameof(SaleDetailModel.SaleId)},
                pd.ProductId AS {nameof(SaleDetailModel.ProductId)},
                pd.ProductCode AS {nameof(SaleDetailModel.ProductCode)},
                pd.ProductName AS {nameof(SaleDetailModel.ProductName)},
                pd.ProductUnitCost AS {nameof(SaleDetailModel.ProductUnitCost)},
                pd.ProductUnitPrice AS {nameof(SaleDetailModel.ProductUnitPrice)},
                pd.ProductUnitId AS {nameof(SaleDetailModel.ProductUnitId)},
                pd.ProductUnitDiscount AS {nameof(SaleDetailModel.ProductUnitDiscount)},
                pd.Quantity AS {nameof(SaleDetailModel.Quantity)},
                pd.BatchNo AS {nameof(SaleDetailModel.BatchNo)},
                pd.ExpiredDate AS {nameof(SaleDetailModel.ExpiredDate)},
                pd.NetUnitPrice AS {nameof(SaleDetailModel.NetUnitPrice)},
                pd.DiscountType AS {nameof(SaleDetailModel.DiscountType)},
                pd.DiscountRate AS {nameof(SaleDetailModel.DiscountRate)},
                pd.DiscountAmount AS {nameof(SaleDetailModel.DiscountAmount)},
                pd.TaxRate AS {nameof(SaleDetailModel.TaxRate)},
                pd.TaxAmount AS {nameof(SaleDetailModel.TaxAmount)},
                pd.TaxMethod AS {nameof(SaleDetailModel.TaxMethod)},
                pd.TotalPrice AS {nameof(SaleDetailModel.TotalPrice)},

                pp.Id AS {nameof(SalePaymentModel.Id)},
                pp.SaleId AS {nameof(SalePaymentModel.SaleId)},
                pp.PaymentDate AS {nameof(SalePaymentModel.PaymentDate)},
                pp.ReceivedAmount AS {nameof(SalePaymentModel.ReceivedAmount)},
                pp.PayingAmount AS {nameof(SalePaymentModel.PayingAmount)},
                pp.ChangeAmount AS {nameof(SalePaymentModel.ChangeAmount)},
                pp.PaymentType AS {nameof(SalePaymentModel.PaymentType)},
                pt.Name AS {nameof(SalePaymentModel.PaymentTypeName)},
                pp.Note AS {nameof(SalePaymentModel.Note)}

            FROM dbo.Sales t
            LEFT JOIN dbo.SaleDetails pd ON pd.SaleId = t.Id
            LEFT JOIN dbo.SalePayments pp ON pp.SaleId = t.Id
            LEFT JOIN dbo.LookupDetails pt ON pt.Id = pp.PaymentType
            WHERE t.Id = @Id
            """;

        var saleDictionary = new Dictionary<Guid, SaleInfoModel>();
        var saleDetailDictionary = new Dictionary<Guid, SaleDetailModel>(); // Track details
        var salePaymentDictionary = new Dictionary<Guid, SalePaymentModel>(); // Track payments

        var result = await connection.QueryAsync<SaleInfoModel, SaleDetailModel, SalePaymentModel, SaleInfoModel>(
            sql,
            (sale, detail, payments) =>
            {
                if (!saleDictionary.TryGetValue(sale.Id, out var saleEntry))
                {
                    saleEntry = sale;
                    saleEntry.SaleDetails = [];
                    saleEntry.PaymentDetails = [];
                    saleDictionary.Add(sale.Id, saleEntry);
                }

                // Add distinct SaleDetails
                if (detail != null && !saleDetailDictionary.ContainsKey(detail.Id))
                {
                    saleEntry.SaleDetails.Add(detail);
                    saleDetailDictionary[detail.Id] = detail; // Keep track of added details
                }

                // Add distinct SalePayments
                if (payments != null && !salePaymentDictionary.ContainsKey(payments.Id))
                {
                    saleEntry.PaymentDetails.Add(payments);
                    salePaymentDictionary[payments.Id] = payments; // Keep track of added payments
                }

                return saleEntry;
            },
            new { request.Id },
            splitOn: "Id, Id, Id"
        );

        var sale = saleDictionary.Values.FirstOrDefault();
        if(sale is not null)
        {
            sale.TotalQuantity = sale.SaleDetails.Count;
            sale.TotalDiscount = sale.SaleDetails.Sum(x => x.DiscountAmount);
            sale.TotalTaxAmount = sale.SaleDetails.Sum(x => x.TaxAmount);
            sale.TotalItems = $"{sale.TotalQuantity} ({sale.SaleDetails.Sum(x => x.Quantity)})";
            sale.CompanyInfo = await commonQueryService.GetCompanyInfoAsync(cancellationToken);
            sale.Customer = await commonQueryService.GetCustomerDetail(sale.CustomerId, cancellationToken);
            return sale;
        }
        else
        {
            return Result.Failure<SaleInfoModel>(Error.Failure(nameof(SaleInfoModel), ErrorMessages.NotFound));
        }
    }
}

