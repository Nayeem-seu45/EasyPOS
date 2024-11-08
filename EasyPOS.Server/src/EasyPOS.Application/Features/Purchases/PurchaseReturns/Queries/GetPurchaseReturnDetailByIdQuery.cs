using EasyPOS.Application.Features.PurchaseReturns.Models;

namespace EasyPOS.Application.Features.PurchaseReturns.Queries;

public record GetPurchaseReturnDetailByIdQuery(Guid Id) : ICacheableQuery<PurchaseReturnInfoModel>
{
    public string CacheKey => $"{CacheKeys.PurchaseReturn}_{Id}";

    public TimeSpan? Expiration => null;

    public bool? AllowCache => false;
}

internal sealed class GetPurchaseReturnDetailByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    ICommonQueryService commonQueryService)
    : IQueryHandler<GetPurchaseReturnDetailByIdQuery, PurchaseReturnInfoModel>
{
    public async Task<Result<PurchaseReturnInfoModel>> Handle(GetPurchaseReturnDetailByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return Result.Failure<PurchaseReturnInfoModel>(Error.Failure(nameof(PurchaseReturnInfoModel), ErrorMessages.NotFound));
        }

        var connection = sqlConnectionFactory.GetOpenConnection();

        // SQL query to get both PurchaseReturn and PurchaseReturnDetails with necessary fields
        var sql = $"""
            SELECT 
                t.Id AS {nameof(PurchaseReturnInfoModel.Id)},
                t.PurchaseReturnDate AS {nameof(PurchaseReturnInfoModel.PurchaseReturnDate)},
                t.ReferenceNo AS {nameof(PurchaseReturnInfoModel.ReferenceNo)},
                t.WarehouseId AS {nameof(PurchaseReturnInfoModel.WarehouseId)},
                t.SupplierId AS {nameof(PurchaseReturnInfoModel.SupplierId)},
                t.PurchaseReturnStatusId AS {nameof(PurchaseReturnInfoModel.PurchaseReturnStatusId)},
                t.AttachmentUrl AS {nameof(PurchaseReturnInfoModel.AttachmentUrl)},
                t.SubTotal AS {nameof(PurchaseReturnInfoModel.SubTotal)},
                t.TaxRate AS {nameof(PurchaseReturnInfoModel.TaxRate)},
                t.TaxAmount AS {nameof(PurchaseReturnInfoModel.TaxAmount)},
                t.DiscountType AS {nameof(PurchaseReturnInfoModel.DiscountType)},
                t.DiscountRate AS {nameof(PurchaseReturnInfoModel.DiscountRate)},
                t.DiscountAmount AS {nameof(PurchaseReturnInfoModel.DiscountAmount)},
                t.ShippingCost AS {nameof(PurchaseReturnInfoModel.ShippingCost)},
                t.GrandTotal AS {nameof(PurchaseReturnInfoModel.GrandTotal)},
                t.Note AS {nameof(PurchaseReturnInfoModel.Note)},

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
            LEFT JOIN dbo.PurchaseReturnPayments pp ON pp.PurchaseReturnId = t.Id
            LEFT JOIN dbo.LookupDetails pt ON pt.Id = pp.PaymentType
            WHERE t.Id = @Id
            """;

        var purchaseDictionary = new Dictionary<Guid, PurchaseReturnInfoModel>();
        var purchaseDetailDictionary = new Dictionary<Guid, PurchaseReturnDetailModel>(); // Track details

        var result = await connection.QueryAsync<PurchaseReturnInfoModel, PurchaseReturnDetailModel, PurchaseReturnInfoModel>(
            sql,
            (purchase, detail) =>
            {
                if (!purchaseDictionary.TryGetValue(purchase.Id, out var purchaseEntry))
                {
                    purchaseEntry = purchase;
                    purchaseEntry.PurchaseReturnDetails = [];
                    purchaseDictionary.Add(purchase.Id, purchaseEntry);
                }

                // Add distinct PurchaseReturnDetails
                if (detail != null && !purchaseDetailDictionary.ContainsKey(detail.Id))
                {
                    purchaseEntry.PurchaseReturnDetails.Add(detail);
                    purchaseDetailDictionary[detail.Id] = detail; // Keep track of added details
                }

                // Add distinct PurchaseReturnPayments
                return purchaseEntry;
            },
            new { request.Id },
            splitOn: "Id, Id, Id"
        );

        var purchase = purchaseDictionary.Values.FirstOrDefault();
        if (purchase is not null)
        {
            purchase.TotalQuantity = purchase.PurchaseReturnDetails.Count;
            purchase.TotalDiscount = purchase.PurchaseReturnDetails.Sum(x => x.DiscountAmount);
            purchase.TotalTaxAmount = purchase.PurchaseReturnDetails.Sum(x => x.TaxAmount);
            purchase.TotalItems = $"{purchase.TotalQuantity} ({purchase.PurchaseReturnDetails.Sum(x => x.ReturnedQuantity)})";

            purchase.CompanyInfo = await commonQueryService.GetCompanyInfoAsync(cancellationToken);
            purchase.Supplier = await commonQueryService.GetSupplierDetail(purchase.SupplierId, cancellationToken);
            return purchase;
        }
        else
        {
            return Result.Failure<PurchaseReturnInfoModel>(Error.Failure(nameof(PurchaseReturnInfoModel), ErrorMessages.NotFound));
        }
    }
}

