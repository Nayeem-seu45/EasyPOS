namespace EasyPOS.Application.Features.Trades.GiftCards.Queries;

[Authorize(Policy = Permissions.GiftCards.View)]
public record GetGiftCardListQuery 
     : DataGridModel, ICacheableQuery<PaginatedResponse<GiftCardModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.GiftCard}_{PageNumber}_{PageSize}";
     
}

internal sealed class GetGiftCardQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetGiftCardListQuery, PaginatedResponse<GiftCardModel>>
{
    public async Task<Result<PaginatedResponse<GiftCardModel>>> Handle(GetGiftCardListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(GiftCardModel.Id)},
                t.CardNo AS {nameof(GiftCardModel.CardNo)},
                t.Amount AS {nameof(GiftCardModel.Amount)},
                t.Expense AS {nameof(GiftCardModel.Expense)},
                t.Balance AS {nameof(GiftCardModel.Balance)},
                t.ExpiredDate AS {nameof(GiftCardModel.ExpiredDate)},
                t.CustomerId AS {nameof(GiftCardModel.CustomerId)},
                t.AllowMultipleTransac AS {nameof(GiftCardModel.AllowMultipleTransac)},
                t.GiftCardType AS {nameof(GiftCardModel.GiftCardType)},
                t.Status AS {nameof(GiftCardModel.Status)}
            FROM dbo.GiftCards AS t
            WHERE 1 = 1
            """;


        return await PaginatedResponse<GiftCardModel>
            .CreateAsync(connection, sql, request);

    }
}


