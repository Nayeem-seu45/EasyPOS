using EasyPOS.Application.Common.Enums;
using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Application.Features.Quotations.Queries;

public record GetQuotationByIdQuery(Guid Id) : ICacheableQuery<UpsertQuotationModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Quotation}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;
}

internal sealed class GetQuotationByIdQueryHandler(ISqlConnectionFactory sqlConnection, ICommonQueryService commonQueryService)
    : IQueryHandler<GetQuotationByIdQuery, UpsertQuotationModel>
{
    public async Task<Result<UpsertQuotationModel>> Handle(GetQuotationByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new UpsertQuotationModel()
            {
                DiscountType = DiscountType.Fixed,
                ShippingCost = 0,
                DiscountAmount = 0,
                QuotationStatusId = await commonQueryService.GetLookupDetailIdAsync((int)QuotationStatus.Pending),
                TaxRate = 0,
                QuotationDate = DateOnly.FromDateTime(DateTime.Now)
            };
        }

        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                -- UpsertQuotationModel fields (master)
                s.Id AS {nameof(UpsertQuotationModel.Id)},
                s.QuotationDate AS {nameof(UpsertQuotationModel.QuotationDate)},
                s.ReferenceNo AS {nameof(UpsertQuotationModel.ReferenceNo)},
                s.WarehouseId AS {nameof(UpsertQuotationModel.WarehouseId)},
                s.CustomerId AS {nameof(UpsertQuotationModel.CustomerId)},
                s.BillerId AS {nameof(UpsertQuotationModel.BillerId)},
                s.AttachmentUrl AS {nameof(UpsertQuotationModel.AttachmentUrl)},
                s.QuotationStatusId AS {nameof(UpsertQuotationModel.QuotationStatusId)},
                s.SubTotal AS {nameof(UpsertQuotationModel.SubTotal)},
                s.TaxRate AS {nameof(UpsertQuotationModel.TaxRate)},
                s.TaxAmount AS {nameof(UpsertQuotationModel.TaxAmount)},
                s.DiscountType AS {nameof(UpsertQuotationModel.DiscountType)},
                s.DiscountRate AS {nameof(UpsertQuotationModel.DiscountRate)},
                s.DiscountAmount AS {nameof(UpsertQuotationModel.DiscountAmount)},
                s.ShippingCost AS {nameof(UpsertQuotationModel.ShippingCost)},
                s.GrandTotal AS {nameof(UpsertQuotationModel.GrandTotal)},
                s.QuotationNote AS {nameof(UpsertQuotationModel.QuotationNote)},
                s.StaffNote AS {nameof(UpsertQuotationModel.StaffNote)},

                -- QuotationDetailModel fields (detail)
                d.Id AS {nameof(QuotationDetailModel.Id)},
                d.QuotationId AS {nameof(QuotationDetailModel.QuotationId)},
                d.ProductId AS {nameof(QuotationDetailModel.ProductId)},
                d.ProductCode AS {nameof(QuotationDetailModel.ProductCode)},
                d.ProductName AS {nameof(QuotationDetailModel.ProductName)},
                d.ProductUnitCost AS {nameof(QuotationDetailModel.ProductUnitCost)},
                d.ProductUnitPrice AS {nameof(QuotationDetailModel.ProductUnitPrice)},
                d.ProductUnitId AS {nameof(QuotationDetailModel.ProductUnitId)},
                d.ProductUnit AS {nameof(QuotationDetailModel.ProductUnit)},
                d.ProductUnitDiscount AS {nameof(QuotationDetailModel.ProductUnitDiscount)},
                d.Quantity AS {nameof(QuotationDetailModel.Quantity)},
                d.BatchNo AS {nameof(QuotationDetailModel.BatchNo)},
                d.ExpiredDate AS {nameof(QuotationDetailModel.ExpiredDate)},
                d.NetUnitPrice AS {nameof(QuotationDetailModel.NetUnitPrice)},
                d.DiscountType AS {nameof(QuotationDetailModel.DiscountType)},
                d.DiscountRate AS {nameof(QuotationDetailModel.DiscountRate)},
                d.DiscountAmount AS {nameof(QuotationDetailModel.DiscountAmount)},
                d.TaxRate AS {nameof(QuotationDetailModel.TaxRate)},
                d.TaxAmount AS {nameof(QuotationDetailModel.TaxAmount)},
                d.TaxMethod AS {nameof(QuotationDetailModel.TaxMethod)},
                d.TotalPrice AS {nameof(QuotationDetailModel.TotalPrice)}
            FROM dbo.Quotations s
            LEFT JOIN dbo.QuotationDetails d ON s.Id = d.QuotationId
            WHERE s.Id = @Id
        """;

        var quotationDictionary = new Dictionary<Guid, UpsertQuotationModel>();

        var quotationModel = await connection.QueryAsync<UpsertQuotationModel, QuotationDetailModel, UpsertQuotationModel>(
            sql,
            (quotation, quotationDetail) =>
            {
                if (!quotationDictionary.TryGetValue(quotation.Id, out var currentQuotation))
                {
                    currentQuotation = quotation;
                    currentQuotation.QuotationDetails = [];
                    quotationDictionary.Add(currentQuotation.Id, currentQuotation);
                }

                if (quotationDetail != null)
                {
                    currentQuotation.QuotationDetails.Add(quotationDetail);
                }

                return currentQuotation;
            },
            new { request.Id },
            splitOn: nameof(QuotationDetailModel.Id)
        );

        var result = quotationModel.FirstOrDefault();

        return result == null
            ? Result.Failure<UpsertQuotationModel>(Error.Failure(nameof(UpsertQuotationModel), ErrorMessages.NotFound))
            : Result.Success(result);
    }
}

