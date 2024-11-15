using EasyPOS.Application.Common.Enums;
using EasyPOS.Application.Features.PurchaseReturns.Models;
using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Application.Features.PurchaseReturns.Queries;

public record GetPurchaseReturnByPurchaseIdQuery(Guid PurchaseId) : ICacheableQuery<PurchaseReturnModel>
{
    public string CacheKey => $"{CacheKeys.PurchaseReturn}_{PurchaseId}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => false;
}

internal sealed class GetPurchaseReturnByPurchaseIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory, 
    ICommonQueryService commonQueryService)
    : IQueryHandler<GetPurchaseReturnByPurchaseIdQuery, PurchaseReturnModel>
{
    public async Task<Result<PurchaseReturnModel>> Handle(GetPurchaseReturnByPurchaseIdQuery request, CancellationToken cancellationToken)
    {

        var connection = sqlConnectionFactory.GetOpenConnection();

        // SQL query to get both PurchaseReturn and PurchaseReturnDetails with necessary fields
        var sql = $"""
            SELECT 
                t.Id AS {nameof(PurchaseReturnModel.PurchaseId)},
                t.ReferenceNo AS {nameof(PurchaseReturnModel.PurchaseReferenceNo)},
                t.WarehouseId AS {nameof(PurchaseReturnModel.WarehouseId)},
                t.SupplierId AS {nameof(PurchaseReturnModel.SupplierId)},
                --t.AttachmentUrl AS {nameof(PurchaseReturnModel.AttachmentUrl)},
                --t.SubTotal AS {nameof(PurchaseReturnModel.SubTotal)},
                --t.TaxRate AS {nameof(PurchaseReturnModel.TaxRate)},
                --t.TaxAmount AS {nameof(PurchaseReturnModel.TaxAmount)},
                --t.DiscountType AS {nameof(PurchaseReturnModel.DiscountType)},
                --t.DiscountRate AS {nameof(PurchaseReturnModel.DiscountRate)},
                --t.DiscountAmount AS {nameof(PurchaseReturnModel.DiscountAmount)},
                --t.ShippingCost AS {nameof(PurchaseReturnModel.ShippingCost)},
                --t.GrandTotal AS {nameof(PurchaseReturnModel.GrandTotal)},
                --t.Note AS {nameof(PurchaseReturnModel.Note)},

                -- PurchaseReturnDetails
                pd.Id AS {nameof(PurchaseReturnDetailModel.Id)},
                pd.ProductId AS {nameof(PurchaseReturnDetailModel.ProductId)},
                pd.ProductCode AS {nameof(PurchaseReturnDetailModel.ProductCode)},
                pd.ProductName AS {nameof(PurchaseReturnDetailModel.ProductName)},
                pd.ProductUnitCost AS {nameof(PurchaseReturnDetailModel.ProductUnitCost)},
                pd.ProductUnitPrice AS {nameof(PurchaseReturnDetailModel.ProductUnitPrice)},
                pd.ProductUnitId AS {nameof(PurchaseReturnDetailModel.ProductUnitId)},
                pd.ProductUnitDiscount AS {nameof(PurchaseReturnDetailModel.ProductUnitDiscount)},
                pd.Quantity AS {nameof(PurchaseReturnDetailModel.PurchasedQuantity)},
                pd.BatchNo AS {nameof(PurchaseReturnDetailModel.BatchNo)},
                pd.ExpiredDate AS {nameof(PurchaseReturnDetailModel.ExpiredDate)},
                pd.NetUnitCost AS {nameof(PurchaseReturnDetailModel.NetUnitCost)},
                pd.DiscountType AS {nameof(PurchaseReturnDetailModel.DiscountType)},
                pd.DiscountRate AS {nameof(PurchaseReturnDetailModel.DiscountRate)},
                pd.DiscountAmount AS {nameof(PurchaseReturnDetailModel.DiscountAmount)},
                pd.TaxRate AS {nameof(PurchaseReturnDetailModel.TaxRate)},
                pd.TaxAmount AS {nameof(PurchaseReturnDetailModel.TaxAmount)},
                pd.TaxMethod AS {nameof(PurchaseReturnDetailModel.TaxMethod)},
                --pd.TotalPrice AS {nameof(PurchaseReturnDetailModel.TotalPrice)}
                0 AS {nameof(PurchaseReturnDetailModel.TotalPrice)}
            FROM dbo.Purchases t
            LEFT JOIN dbo.PurchaseDetails pd ON pd.PurchaseId = t.Id
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
            new { Id = request.PurchaseId },
            splitOn: "Id"
        );

        var purchaseReturn = purchaseDictionary.Values.FirstOrDefault();
        purchaseReturn.ReturnDate = DateOnly.FromDateTime(DateTime.Now);
        purchaseReturn.TaxRate = 0; // No Tax Rate
        purchaseReturn.DiscountType = DiscountType.Fixed;
        var returnStatusId = await commonQueryService.GetLookupDetailIdAsync((int)PurchaseReturnStatus.Completed, cancellationToken);
        if (!returnStatusId.IsNullOrEmpty())
        {
            purchaseReturn.ReturnStatusId = returnStatusId.Value;
        }

        return purchaseReturn != null ? Result.Success(purchaseReturn) : Result.Failure<PurchaseReturnModel>(Error.Failure(nameof(PurchaseReturnModel), ErrorMessages.NotFound));
    }
}

