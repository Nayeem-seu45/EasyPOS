namespace EasyPOS.Application.Features.Quotations.Queries;

public record GetQuotationDetailByIdQuery(Guid Id) : ICacheableQuery<QuotationInfoModel>
{
    public string CacheKey => $"{CacheKeys.Quotation}_{Id}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => false;
}

internal sealed class GetQuotationDetailByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    ICommonQueryService commonQueryService)
    : IQueryHandler<GetQuotationDetailByIdQuery, QuotationInfoModel>
{
    public async Task<Result<QuotationInfoModel>> Handle(GetQuotationDetailByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return Result.Failure<QuotationInfoModel>(Error.Failure(nameof(QuotationInfoModel), ErrorMessages.NotFound));
        }

        var connection = sqlConnectionFactory.GetOpenConnection();

        // SQL query to get both Quotation and QuotationDetails with necessary fields
        var sql = $"""
            SELECT 
                t.Id AS {nameof(QuotationInfoModel.Id)},
                t.QuotationDate AS {nameof(QuotationInfoModel.QuotationDate)},
                t.ReferenceNo AS {nameof(QuotationInfoModel.ReferenceNo)},
                t.WarehouseId AS {nameof(QuotationInfoModel.WarehouseId)},
                t.CustomerId AS {nameof(QuotationInfoModel.CustomerId)},
                t.QuotationStatusId AS {nameof(QuotationInfoModel.QuotationStatusId)},
                t.AttachmentUrl AS {nameof(QuotationInfoModel.AttachmentUrl)},
                t.SubTotal AS {nameof(QuotationInfoModel.SubTotal)},
                t.TaxRate AS {nameof(QuotationInfoModel.TaxRate)},
                t.TaxAmount AS {nameof(QuotationInfoModel.TaxAmount)},
                t.DiscountType AS {nameof(QuotationInfoModel.DiscountType)},
                t.DiscountRate AS {nameof(QuotationInfoModel.DiscountRate)},
                t.DiscountAmount AS {nameof(QuotationInfoModel.DiscountAmount)},
                t.ShippingCost AS {nameof(QuotationInfoModel.ShippingCost)},
                t.GrandTotal AS {nameof(QuotationInfoModel.GrandTotal)},
                t.QuotationNote AS {nameof(QuotationInfoModel.QuotationNote)},

                -- QuotationDetails
                pd.Id AS {nameof(QuotationDetailModel.Id)},
                pd.QuotationId AS {nameof(QuotationDetailModel.QuotationId)},
                pd.ProductId AS {nameof(QuotationDetailModel.ProductId)},
                pd.ProductCode AS {nameof(QuotationDetailModel.ProductCode)},
                pd.ProductName AS {nameof(QuotationDetailModel.ProductName)},
                pd.ProductUnitCost AS {nameof(QuotationDetailModel.ProductUnitCost)},
                pd.ProductUnitPrice AS {nameof(QuotationDetailModel.ProductUnitPrice)},
                pd.ProductUnitId AS {nameof(QuotationDetailModel.ProductUnitId)},
                pd.ProductUnitDiscount AS {nameof(QuotationDetailModel.ProductUnitDiscount)},
                pd.Quantity AS {nameof(QuotationDetailModel.Quantity)},
                pd.BatchNo AS {nameof(QuotationDetailModel.BatchNo)},
                pd.ExpiredDate AS {nameof(QuotationDetailModel.ExpiredDate)},
                pd.NetUnitPrice AS {nameof(QuotationDetailModel.NetUnitPrice)},
                pd.DiscountType AS {nameof(QuotationDetailModel.DiscountType)},
                pd.DiscountRate AS {nameof(QuotationDetailModel.DiscountRate)},
                pd.DiscountAmount AS {nameof(QuotationDetailModel.DiscountAmount)},
                pd.TaxRate AS {nameof(QuotationDetailModel.TaxRate)},
                pd.TaxAmount AS {nameof(QuotationDetailModel.TaxAmount)},
                pd.TaxMethod AS {nameof(QuotationDetailModel.TaxMethod)},
                pd.TotalPrice AS {nameof(QuotationDetailModel.TotalPrice)}


            FROM dbo.Quotations t
            LEFT JOIN dbo.QuotationDetails pd ON pd.QuotationId = t.Id
            LEFT JOIN dbo.LookupDetails pt ON pt.Id = pp.PaymentType
            WHERE t.Id = @Id
            """;

        var quotationDictionary = new Dictionary<Guid, QuotationInfoModel>();
        var quotationDetailDictionary = new Dictionary<Guid, QuotationDetailModel>(); // Track details

        var result = await connection.QueryAsync<QuotationInfoModel, QuotationDetailModel, QuotationInfoModel>(
            sql,
            (quotation, detail) =>
            {
                if (!quotationDictionary.TryGetValue(quotation.Id, out var quotationEntry))
                {
                    quotationEntry = quotation;
                    quotationEntry.QuotationDetails = [];
                    quotationDictionary.Add(quotation.Id, quotationEntry);
                }

                // Add distinct QuotationDetails
                if (detail != null && !quotationDetailDictionary.ContainsKey(detail.Id))
                {
                    quotationEntry.QuotationDetails.Add(detail);
                    quotationDetailDictionary[detail.Id] = detail; // Keep track of added details
                }

                return quotationEntry;
            },
            new { request.Id },
            splitOn: "Id, Id, Id"
        );

        var quotation = quotationDictionary.Values.FirstOrDefault();
        if (quotation is not null)
        {
            quotation.TotalQuantity = quotation.QuotationDetails.Count;
            quotation.TotalDiscount = quotation.QuotationDetails.Sum(x => x.DiscountAmount);
            quotation.TotalTaxAmount = quotation.QuotationDetails.Sum(x => x.TaxAmount);
            quotation.TotalItems = $"{quotation.TotalQuantity} ({quotation.QuotationDetails.Sum(x => x.Quantity)})";
            quotation.CompanyInfo = await commonQueryService.GetCompanyInfoAsync(cancellationToken);
            quotation.Customer = await commonQueryService.GetCustomerDetail(quotation.CustomerId, cancellationToken);
            return quotation;
        }
        else
        {
            return Result.Failure<QuotationInfoModel>(Error.Failure(nameof(QuotationInfoModel), ErrorMessages.NotFound));
        }
    }
}

