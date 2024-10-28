namespace EasyPOS.Application.Features.Accounting.Expenses.Queries;

public record GetExpenseByIdQuery(Guid Id) : ICacheableQuery<ExpenseModel>
{
    [JsonIgnore]
    public string CacheKey => $"{CacheKeys.Expense}_{Id}";
    [JsonIgnore]
    public TimeSpan? Expiration => null;
    public bool? AllowCache => true;
}

internal sealed class GetExpenseByIdQueryHandler(ISqlConnectionFactory sqlConnection)
     : IQueryHandler<GetExpenseByIdQuery, ExpenseModel>
{

    public async Task<Result<ExpenseModel>> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.IsNullOrEmpty())
        {
            return new ExpenseModel();
        }
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT
                t.Id AS {nameof(ExpenseModel.Id)},
                t.ExpenseDate AS {nameof(ExpenseModel.ExpenseDate)},
                t.Title AS {nameof(ExpenseModel.Title)},
                t.ReferenceNo AS {nameof(ExpenseModel.ReferenceNo)},
                t.WarehouseId AS {nameof(ExpenseModel.WarehouseId)},
                t.CategoryId AS {nameof(ExpenseModel.CategoryId)},
                t.Amount AS {nameof(ExpenseModel.Amount)},
                t.AccountId AS {nameof(ExpenseModel.AccountId)},
                t.Description AS {nameof(ExpenseModel.Description)},
                t.AttachmentUrl AS {nameof(ExpenseModel.AttachmentUrl)}
            FROM dbo.Expenses AS t
            WHERE t.Id = @Id
            """;


        return await connection.QueryFirstOrDefaultAsync<ExpenseModel>(sql, new { request.Id });
    }
}

