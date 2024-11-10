using EasyPOS.Application.Common.Enums;
using EasyPOS.Application.Features.SaleReturns.Models;
using EasyPOS.Domain.Common.Enums;

namespace EasyPOS.Application.Features.SaleReturns.Queries;

public record GetSaleReturnByIdQuery(Guid Id) : ICacheableQuery<UpsertSaleReturnModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.SaleReturn}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => false;
}

internal sealed class GetSaleReturnByIdQueryHandler(ISqlConnectionFactory sqlConnection, ICommonQueryService commonQueryService)
    : IQueryHandler<GetSaleReturnByIdQuery, UpsertSaleReturnModel>
{
    public async Task<Result<UpsertSaleReturnModel>> Handle(GetSaleReturnByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new UpsertSaleReturnModel()
            {
                DiscountType = DiscountType.Fixed,
                ShippingCost = 0,
                DiscountAmount = 0,
                ReturnStatusId = await commonQueryService.GetLookupDetailIdAsync((int)SaleReturnStatus.Received),
                PaymentStatusId = await commonQueryService.GetLookupDetailIdAsync((int)PaymentStatus.Pending),
                TaxRate = 0,
                ReturnDate = DateOnly.FromDateTime(DateTime.Now)
            };
        }

        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                -- UpsertSaleReturnModel fields (master)
                s.Id AS {nameof(UpsertSaleReturnModel.Id)},
                s.ReturnDate AS {nameof(UpsertSaleReturnModel.ReturnDate)},
                s.ReferenceNo AS {nameof(UpsertSaleReturnModel.ReferenceNo)},
                s.WarehouseId AS {nameof(UpsertSaleReturnModel.WarehouseId)},
                s.CustomerId AS {nameof(UpsertSaleReturnModel.CustomerId)},
                s.BillerId AS {nameof(UpsertSaleReturnModel.BillerId)},
                s.AttachmentUrl AS {nameof(UpsertSaleReturnModel.AttachmentUrl)},
                s.ReturnStatusId AS {nameof(UpsertSaleReturnModel.ReturnStatusId)},
                s.PaymentStatusId AS {nameof(UpsertSaleReturnModel.PaymentStatusId)},
                s.SubTotal AS {nameof(UpsertSaleReturnModel.SubTotal)},
                s.TaxRate AS {nameof(UpsertSaleReturnModel.TaxRate)},
                s.TaxAmount AS {nameof(UpsertSaleReturnModel.TaxAmount)},
                s.DiscountType AS {nameof(UpsertSaleReturnModel.DiscountType)},
                s.DiscountRate AS {nameof(UpsertSaleReturnModel.DiscountRate)},
                s.DiscountAmount AS {nameof(UpsertSaleReturnModel.DiscountAmount)},
                s.ShippingCost AS {nameof(UpsertSaleReturnModel.ShippingCost)},
                s.GrandTotal AS {nameof(UpsertSaleReturnModel.GrandTotal)},
                s.ReturnNote AS {nameof(UpsertSaleReturnModel.ReturnNote)},
                s.StaffNote AS {nameof(UpsertSaleReturnModel.StaffNote)},

                -- SaleReturnDetailModel fields (detail)
                d.Id AS {nameof(SaleReturnDetailModel.Id)},
                d.SaleReturnId AS {nameof(SaleReturnDetailModel.SaleReturnId)},
                d.ProductId AS {nameof(SaleReturnDetailModel.ProductId)},
                d.ProductCode AS {nameof(SaleReturnDetailModel.ProductCode)},
                d.ProductName AS {nameof(SaleReturnDetailModel.ProductName)},
                d.ProductUnitCost AS {nameof(SaleReturnDetailModel.ProductUnitCost)},
                d.ProductUnitPrice AS {nameof(SaleReturnDetailModel.ProductUnitPrice)},
                d.ProductUnitId AS {nameof(SaleReturnDetailModel.ProductUnitId)},
                d.ProductUnit AS {nameof(SaleReturnDetailModel.ProductUnit)},
                d.ProductUnitDiscount AS {nameof(SaleReturnDetailModel.ProductUnitDiscount)},
                d.SoldQuantity AS {nameof(SaleReturnDetailModel.SoldQuantity)},
                d.ReturnedQuantity AS {nameof(SaleReturnDetailModel.ReturnedQuantity)},
                d.BatchNo AS {nameof(SaleReturnDetailModel.BatchNo)},
                d.ExpiredDate AS {nameof(SaleReturnDetailModel.ExpiredDate)},
                d.NetUnitPrice AS {nameof(SaleReturnDetailModel.NetUnitPrice)},
                d.DiscountType AS {nameof(SaleReturnDetailModel.DiscountType)},
                d.DiscountRate AS {nameof(SaleReturnDetailModel.DiscountRate)},
                d.DiscountAmount AS {nameof(SaleReturnDetailModel.DiscountAmount)},
                d.TaxRate AS {nameof(SaleReturnDetailModel.TaxRate)},
                d.TaxAmount AS {nameof(SaleReturnDetailModel.TaxAmount)},
                d.TaxMethod AS {nameof(SaleReturnDetailModel.TaxMethod)},
                d.TotalPrice AS {nameof(SaleReturnDetailModel.TotalPrice)}
            FROM dbo.SaleReturns s
            LEFT JOIN dbo.SaleReturnDetails d ON s.Id = d.SaleReturnId
            WHERE s.Id = @Id
        """;

        var saleDictionary = new Dictionary<Guid, UpsertSaleReturnModel>();

        var saleModel = await connection.QueryAsync<UpsertSaleReturnModel, SaleReturnDetailModel, UpsertSaleReturnModel>(
            sql,
            (sale, saleDetail) =>
            {
                if (!saleDictionary.TryGetValue(sale.Id, out var currentSaleReturn))
                {
                    currentSaleReturn = sale;
                    currentSaleReturn.SaleReturnDetails = [];
                    saleDictionary.Add(currentSaleReturn.Id, currentSaleReturn);
                }

                if (saleDetail != null)
                {
                    currentSaleReturn.SaleReturnDetails.Add(saleDetail);
                }

                return currentSaleReturn;
            },
            new { request.Id },
            splitOn: nameof(SaleReturnDetailModel.Id)
        );

        var result = saleModel.FirstOrDefault();

        return result == null
            ? Result.Failure<UpsertSaleReturnModel>(Error.Failure(nameof(UpsertSaleReturnModel), ErrorMessages.NotFound))
            : Result.Success(result);
    }
}

