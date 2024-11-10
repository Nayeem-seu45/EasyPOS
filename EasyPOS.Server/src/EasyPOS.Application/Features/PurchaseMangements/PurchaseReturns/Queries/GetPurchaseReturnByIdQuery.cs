using EasyPOS.Application.Features.PurchaseReturns.Models;

namespace EasyPOS.Application.Features.PurchaseReturns.Queries;

public record GetPurchaseReturnByIdQuery(Guid Id) : ICacheableQuery<PurchaseReturnModel>
{
    public string CacheKey => $"{CacheKeys.PurchaseReturn}_{Id}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => false;
}

internal sealed class GetPurchaseReturnByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetPurchaseReturnByIdQuery, PurchaseReturnModel>
{
    public async Task<Result<PurchaseReturnModel>> Handle(GetPurchaseReturnByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return new PurchaseReturnModel();
        }

        var connection = sqlConnectionFactory.GetOpenConnection();

        // SQL query to get both PurchaseReturn and PurchaseReturnDetails with necessary fields
        var sql = $"""
            SELECT 
                t.Id AS {nameof(PurchaseReturnModel.Id)},
                t.ReturnDate AS {nameof(PurchaseReturnModel.ReturnDate)},
                t.ReferenceNo AS {nameof(PurchaseReturnModel.ReferenceNo)},
                t.WarehouseId AS {nameof(PurchaseReturnModel.WarehouseId)},
                t.SupplierId AS {nameof(PurchaseReturnModel.SupplierId)},
                t.ReturnStatusId AS {nameof(PurchaseReturnModel.ReturnStatusId)},
                t.AttachmentUrl AS {nameof(PurchaseReturnModel.AttachmentUrl)},
                t.SubTotal AS {nameof(PurchaseReturnModel.SubTotal)},
                t.TaxRate AS {nameof(PurchaseReturnModel.TaxRate)},
                t.TaxAmount AS {nameof(PurchaseReturnModel.TaxAmount)},
                t.DiscountType AS {nameof(PurchaseReturnModel.DiscountType)},
                t.DiscountRate AS {nameof(PurchaseReturnModel.DiscountRate)},
                t.DiscountAmount AS {nameof(PurchaseReturnModel.DiscountAmount)},
                t.ShippingCost AS {nameof(PurchaseReturnModel.ShippingCost)},
                t.GrandTotal AS {nameof(PurchaseReturnModel.GrandTotal)},
                t.Note AS {nameof(PurchaseReturnModel.Note)},

                -- PurchaseReturnDetails
                pd.Id AS {nameof(PurchaseReturnDetailModel.Id)},
                pd.PurchaseReturnId AS {nameof(PurchaseReturnDetailModel.PurchaseReturnId)},
                pd.ProductId AS {nameof(PurchaseReturnDetailModel.ProductId)},
                pd.ProductCode AS {nameof(PurchaseReturnDetailModel.ProductCode)},
                pd.ProductName AS {nameof(PurchaseReturnDetailModel.ProductName)},
                pd.ProductUnitCost AS {nameof(PurchaseReturnDetailModel.ProductUnitCost)},
                pd.ProductUnitPrice AS {nameof(PurchaseReturnDetailModel.ProductUnitPrice)},
                pd.ProductUnitId AS {nameof(PurchaseReturnDetailModel.ProductUnitId)},
                pd.ProductUnitDiscount AS {nameof(PurchaseReturnDetailModel.ProductUnitDiscount)},
                pd.PurchasedQuantity AS {nameof(PurchaseReturnDetailModel.PurchasedQuantity)},
                pd.ReturnedQuantity AS {nameof(PurchaseReturnDetailModel.ReturnedQuantity)},
                pd.BatchNo AS {nameof(PurchaseReturnDetailModel.BatchNo)},
                pd.ExpiredDate AS {nameof(PurchaseReturnDetailModel.ExpiredDate)},
                pd.NetUnitCost AS {nameof(PurchaseReturnDetailModel.NetUnitCost)},
                pd.DiscountType AS {nameof(PurchaseReturnDetailModel.DiscountType)},
                pd.DiscountRate AS {nameof(PurchaseReturnDetailModel.DiscountRate)},
                pd.DiscountAmount AS {nameof(PurchaseReturnDetailModel.DiscountAmount)},
                pd.TaxRate AS {nameof(PurchaseReturnDetailModel.TaxRate)},
                pd.TaxAmount AS {nameof(PurchaseReturnDetailModel.TaxAmount)},
                pd.TaxMethod AS {nameof(PurchaseReturnDetailModel.TaxMethod)},
                pd.TotalPrice AS {nameof(PurchaseReturnDetailModel.TotalPrice)}
            FROM dbo.PurchaseReturns t
            LEFT JOIN dbo.PurchaseReturnDetails pd ON pd.PurchaseReturnId = t.Id
            WHERE t.Id = @Id
            """;

        var purchaseDictionary = new Dictionary<Guid, PurchaseReturnModel>();

        var result = await connection.QueryAsync<PurchaseReturnModel, PurchaseReturnDetailModel, PurchaseReturnModel>(
            sql,
            (purchase, detail) =>
            {
                if (!purchaseDictionary.TryGetValue(purchase.Id, out var purchaseEntry))
                {
                    purchaseEntry = purchase;
                    purchaseEntry.PurchaseReturnDetails = [];
                    purchaseDictionary.Add(purchase.Id, purchaseEntry);
                }

                if (detail != null)
                {
                    purchaseEntry.PurchaseReturnDetails.Add(detail);
                }

                return purchaseEntry;
            },
            new { request.Id },
            splitOn: "Id"
        );

        var purchase = purchaseDictionary.Values.FirstOrDefault();
        return purchase != null ? Result.Success(purchase) : Result.Failure<PurchaseReturnModel>(Error.Failure(nameof(PurchaseReturnModel), ErrorMessages.NotFound));
    }
}

