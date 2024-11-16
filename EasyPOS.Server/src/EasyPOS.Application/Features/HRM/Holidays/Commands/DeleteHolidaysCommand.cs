namespace EasyPOS.Application.Features.HRM.Holidays.Commands;

public record DeleteHolidaysCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Holiday;
}

internal sealed class DeleteHolidaysCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteHolidaysCommand>

{
    public async Task<Result> Handle(DeleteHolidaysCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.Holidays
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}