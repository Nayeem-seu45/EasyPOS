namespace EasyPOS.Application.Features.Sales.GiftCards.Queries;

public record GetGiftCardByIdQuery(Guid Id) : ICacheableQuery<GiftCardModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.GiftCard}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetGiftCardByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetGiftCardByIdQuery, GiftCardModel>
{

    public async Task<Result<GiftCardModel>> Handle(GetGiftCardByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new GiftCardModel();
        }
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
                t.Status AS {nameof(GiftCardModel.Status)}
            FROM dbo.GiftCards AS t
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<GiftCardModel>(sql, new { request.Id });
    }
}

