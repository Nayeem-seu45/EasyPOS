namespace EasyPOS.Application.Features.Quotations.Queries;

[Authorize(Policy = Permissions.Quotations.View)]
public record GetQuotationListQuery
     : DataGridModel, ICacheableQuery<PaginatedResponse<QuotationModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.Quotation}_{PageNumber}_{PageSize}";
}

internal sealed class GetQuotationQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetQuotationListQuery, PaginatedResponse<QuotationModel>>
{
    public async Task<Result<PaginatedResponse<QuotationModel>>> Handle(GetQuotationListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(QuotationModel.Id)},
                t.QuotationDate As {nameof(QuotationModel.QuotationDate)},
                t.ReferenceNo AS {nameof(QuotationModel.ReferenceNo)},
                t.WarehouseId AS {nameof(QuotationModel.WarehouseId)},
                t.CustomerId AS {nameof(QuotationModel.CustomerId)},
                t.AttachmentUrl AS {nameof(QuotationModel.AttachmentUrl)},
                t.QuotationStatusId AS {nameof(QuotationModel.QuotationStatusId)},
                t.TaxRate AS {nameof(QuotationModel.TaxRate)},
                t.TaxAmount AS {nameof(QuotationModel.TaxAmount)},
                t.DiscountAmount AS {nameof(QuotationModel.DiscountAmount)},
                t.ShippingCost AS {nameof(QuotationModel.ShippingCost)},
                t.GrandTotal AS {nameof(QuotationModel.GrandTotal)},
                t.QuotationNote AS {nameof(QuotationModel.QuotationNote)},
                w.Name AS {nameof(QuotationModel.WarehouseName)},
                c.Name AS {nameof(QuotationModel.CustomerName)},
                ss.Name AS {nameof(QuotationModel.QuotationStatus)}
            FROM dbo.Quotations AS t
            LEFT JOIN dbo.Warehouses w ON w.Id = t.WarehouseId
            LEFT JOIN dbo.Customers c ON c.Id = t.CustomerId
            LEFT JOIN dbo.LookupDetails ss ON ss.Id = t.QuotationStatusId
            WHERE 1 = 1
            """;


        return await PaginatedResponse<QuotationModel>
            .CreateAsync(connection, sql, request);

    }
}


