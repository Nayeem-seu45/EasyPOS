namespace EasyPOS.Application.Features.ProductTransfers.Queries;

public record GetProductTransferDetailByIdQuery(Guid Id) : ICacheableQuery<ProductTransferInfoModel>
{
    public string CacheKey => $"{CacheKeys.ProductTransfer}_{Id}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => false;
}

internal sealed class GetProductTransferDetailByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    ICommonQueryService commonQueryService)
    : IQueryHandler<GetProductTransferDetailByIdQuery, ProductTransferInfoModel>
{
    public async Task<Result<ProductTransferInfoModel>> Handle(GetProductTransferDetailByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return Result.Failure<ProductTransferInfoModel>(Error.Failure(nameof(ProductTransferInfoModel), ErrorMessages.NotFound));
        }

        var connection = sqlConnectionFactory.GetOpenConnection();

        // SQL query to get both ProductTransfer and ProductTransferDetails with necessary fields
        var sql = $"""
            SELECT 
                t.Id AS {nameof(ProductTransferInfoModel.Id)},
                t.TransferDate AS {nameof(ProductTransferInfoModel.TransferDate)},
                t.ReferenceNo AS {nameof(ProductTransferInfoModel.ReferenceNo)},
                t.FromWarehouseId AS {nameof(ProductTransferInfoModel.FromWarehouseId)},
                t.ToWarehouseId AS {nameof(ProductTransferInfoModel.ToWarehouseId)},
                t.TransferStatusId AS {nameof(ProductTransferInfoModel.TransferStatusId)},
                t.AttachmentUrl AS {nameof(ProductTransferInfoModel.AttachmentUrl)},
                t.SubTotal AS {nameof(ProductTransferInfoModel.SubTotal)},
                t.TaxRate AS {nameof(ProductTransferInfoModel.TaxRate)},
                t.TaxAmount AS {nameof(ProductTransferInfoModel.TaxAmount)},
                t.DiscountType AS {nameof(ProductTransferInfoModel.DiscountType)},
                t.DiscountRate AS {nameof(ProductTransferInfoModel.DiscountRate)},
                t.DiscountAmount AS {nameof(ProductTransferInfoModel.DiscountAmount)},
                t.ShippingCost AS {nameof(ProductTransferInfoModel.ShippingCost)},
                t.GrandTotal AS {nameof(ProductTransferInfoModel.GrandTotal)},
                t.Note AS {nameof(ProductTransferInfoModel.Note)},
                fw.Name AS {nameof(ProductTransferInfoModel.FromWarehouse)},
                fw.PhoneNo AS {nameof(ProductTransferInfoModel.FromWarehousePhone)},
                fw.Email AS {nameof(ProductTransferInfoModel.FromWarehouseEmail)},
                tw.Name AS {nameof(ProductTransferInfoModel.ToWarehouse)},
                tw.PhoneNo AS {nameof(ProductTransferInfoModel.ToWarehousePhone)},
                tw.Email AS {nameof(ProductTransferInfoModel.ToWarehouseEmail)},
                stat.Name AS {nameof(ProductTransferInfoModel.TransferStatus)},

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
            LEFT JOIN dbo.Warehouses fw ON fw.Id = t.FromWarehouseId
            LEFT JOIN dbo.Warehouses tw ON tw.Id = t.ToWarehouseId
            LEFT JOIN dbo.LookupDetails stat ON stat.Id = t.TransferStatusId
            WHERE t.Id = @Id
            """;

        var productTransferDictionary = new Dictionary<Guid, ProductTransferInfoModel>();
        var productTransferDetailDictionary = new Dictionary<Guid, ProductTransferDetailModel>(); // Track details

        var result = await connection.QueryAsync<ProductTransferInfoModel, ProductTransferDetailModel, ProductTransferInfoModel>(
            sql,
            (productTransfer, detail) =>
            {
                if (!productTransferDictionary.TryGetValue(productTransfer.Id, out var productTransferEntry))
                {
                    productTransferEntry = productTransfer;
                    productTransferEntry.ProductTransferDetails = [];
                    productTransferDictionary.Add(productTransfer.Id, productTransferEntry);
                }

                // Add distinct ProductTransferDetails
                if (detail != null && !productTransferDetailDictionary.ContainsKey(detail.Id))
                {
                    productTransferEntry.ProductTransferDetails.Add(detail);
                    productTransferDetailDictionary[detail.Id] = detail; // Keep track of added details
                }

                return productTransferEntry;
            },
            new { request.Id },
            splitOn: "Id, Id, Id"
        );

        var productTransfer = productTransferDictionary.Values.FirstOrDefault();
        if (productTransfer is not null)
        {
            productTransfer.TotalQuantity = productTransfer.ProductTransferDetails.Count;
            productTransfer.TotalDiscount = productTransfer.ProductTransferDetails.Sum(x => x.DiscountAmount);
            productTransfer.TotalTaxAmount = productTransfer.ProductTransferDetails.Sum(x => x.TaxAmount);
            productTransfer.TotalItems = $"{productTransfer.TotalQuantity} ({productTransfer.ProductTransferDetails?.Sum(x => x.Quantity)})";
            return productTransfer;
        }
        else
        {
            return Result.Failure<ProductTransferInfoModel>(Error.Failure(nameof(ProductTransferInfoModel), ErrorMessages.NotFound));
        }
    }
}

