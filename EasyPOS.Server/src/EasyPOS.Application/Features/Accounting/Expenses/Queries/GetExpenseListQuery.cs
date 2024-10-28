namespace EasyPOS.Application.Features.Accounting.Expenses.Queries;

[Authorize(Policy = Permissions.Expenses.View)]
public record GetExpenseListQuery 
     : DataGridModel, ICacheableQuery<PaginatedResponse<ExpenseModel>>
{
    [JsonInclude]
    public string CacheKey => $"{CacheKeys.Expense}_{PageNumber}_{PageSize}";
     
}

internal sealed class GetExpenseQueryHandler(ISqlConnectionFactory sqlConnection) 
     : IQueryHandler<GetExpenseListQuery, PaginatedResponse<ExpenseModel>>
{
    public async Task<Result<PaginatedResponse<ExpenseModel>>> Handle(GetExpenseListQuery request, CancellationToken cancellationToken)
    {
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
            WHERE 1 = 1
            """;


        return await PaginatedResponse<ExpenseModel>
            .CreateAsync(connection, sql, request);

    }
}


