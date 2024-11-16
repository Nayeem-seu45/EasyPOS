namespace EasyPOS.Application.Features.HRM.WorkingShifts.Commands;

public record DeleteWorkingShiftsCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.WorkingShift;
}

internal sealed class DeleteWorkingShiftsCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteWorkingShiftsCommand>

{
    public async Task<Result> Handle(DeleteWorkingShiftsCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.WorkingShifts
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}