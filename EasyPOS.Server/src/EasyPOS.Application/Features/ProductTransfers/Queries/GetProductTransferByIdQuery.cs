namespace EasyPOS.Application.Features.ProductTransfers.Queries;

public record GetProductTransferByIdQuery(Guid Id) : ICacheableQuery<ProductTransferModel>
{
    public string CacheKey => $"{CacheKeys.ProductTransfer}_{Id}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => false;
}

internal sealed class GetProductTransferByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetProductTransferByIdQuery, ProductTransferModel>
{
    public async Task<Result<ProductTransferModel>> Handle(GetProductTransferByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return new ProductTransferModel();
        }

        var connection = sqlConnectionFactory.GetOpenConnection();

        // SQL query to get both ProductTransfer and ProductTransferDetails with necessary fields
        var sql = $"""
            SELECT 
                t.Id AS {nameof(ProductTransferModel.Id)},
                t.ProductTransferDate AS {nameof(ProductTransferModel.TransferDate)},
                t.ReferenceNo AS {nameof(ProductTransferModel.ReferenceNo)},
                t.FromWarehouseId AS {nameof(ProductTransferModel.FromWarehouseId)},
                t.ToWarehouseId AS {nameof(ProductTransferModel.ToWarehouseId)},
                t.SupplierId AS {nameof(ProductTransferModel.SupplierId)},
                t.ProductTransferStatusId AS {nameof(ProductTransferModel.TransferStatusId)},
                t.AttachmentUrl AS {nameof(ProductTransferModel.AttachmentUrl)},
                t.SubTotal AS {nameof(ProductTransferModel.SubTotal)},
                t.TaxRate AS {nameof(ProductTransferModel.TaxRate)},
                t.TaxAmount AS {nameof(ProductTransferModel.TaxAmount)},
                t.DiscountType AS {nameof(ProductTransferModel.DiscountType)},
                t.DiscountRate AS {nameof(ProductTransferModel.DiscountRate)},
                t.DiscountAmount AS {nameof(ProductTransferModel.DiscountAmount)},
                t.ShippingCost AS {nameof(ProductTransferModel.ShippingCost)},
                t.GrandTotal AS {nameof(ProductTransferModel.GrandTotal)},
                t.Note AS {nameof(ProductTransferModel.Note)},

                -- ProductTransferDetails
                pd.Id AS {nameof(ProductTransferDetailModel.Id)},
                pd.ProductTransferId AS {nameof(ProductTransferDetailModel.ProductTransferId)},
                pd.ProductId AS {nameof(ProductTransferDetailModel.ProductId)},
                pd.ProductCode AS {nameof(ProductTransferDetailModel.ProductCode)},
                pd.ProductName AS {nameof(ProductTransferDetailModel.ProductName)},
                pd.ProductUnitCost AS {nameof(ProductTransferDetailModel.ProductUnitCost)},
                pd.ProductUnitPrice AS {nameof(ProductTransferDetailModel.ProductUnitPrice)},
                pd.ProductUnitId AS {nameof(ProductTransferDetailModel.ProductUnitId)},
                pd.ProductUnitDiscount AS {nameof(ProductTransferDetailModel.ProductUnitDiscount)},
                pd.Quantity AS {nameof(ProductTransferDetailModel.Quantity)},
                pd.BatchNo AS {nameof(ProductTransferDetailModel.BatchNo)},
                pd.ExpiredDate AS {nameof(ProductTransferDetailModel.ExpiredDate)},
                pd.NetUnitCost AS {nameof(ProductTransferDetailModel.NetUnitCost)},
                pd.DiscountType AS {nameof(ProductTransferDetailModel.DiscountType)},
                pd.DiscountRate AS {nameof(ProductTransferDetailModel.DiscountRate)},
                pd.DiscountAmount AS {nameof(ProductTransferDetailModel.DiscountAmount)},
                pd.TaxRate AS {nameof(ProductTransferDetailModel.TaxRate)},
                pd.TaxAmount AS {nameof(ProductTransferDetailModel.TaxAmount)},
                pd.TaxMethod AS {nameof(ProductTransferDetailModel.TaxMethod)},
                pd.TotalPrice AS {nameof(ProductTransferDetailModel.TotalPrice)}
            FROM dbo.ProductTransfers t
            LEFT JOIN dbo.ProductTransferDetails pd ON pd.ProductTransferId = t.Id
            WHERE t.Id = @Id
            """;

        var purchaseDictionary = new Dictionary<Guid, ProductTransferModel>();

        var result = await connection.QueryAsync<ProductTransferModel, ProductTransferDetailModel, ProductTransferModel>(
            sql,
            (purchase, detail) =>
            {
                if (!purchaseDictionary.TryGetValue(purchase.Id, out var purchaseEntry))
                {
                    purchaseEntry = purchase;
                    purchaseEntry.ProductTransferDetails = [];
                    purchaseDictionary.Add(purchase.Id, purchaseEntry);
                }

                if (detail != null)
                {
                    purchaseEntry.ProductTransferDetails.Add(detail);
                }

                return purchaseEntry;
            },
            new { request.Id },
            splitOn: "Id"
        );

        var purchase = purchaseDictionary.Values.FirstOrDefault();
        return purchase != null ? Result.Success(purchase) : Result.Failure<ProductTransferModel>(Error.Failure(nameof(ProductTransferModel), ErrorMessages.NotFound));
    }
}

