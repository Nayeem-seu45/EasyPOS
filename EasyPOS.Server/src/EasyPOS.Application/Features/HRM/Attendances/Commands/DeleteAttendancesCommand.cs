namespace EasyPOS.Application.Features.HRM.Attendances.Commands;

public record DeleteAttendancesCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Attendance;
}

internal sealed class DeleteAttendancesCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteAttendancesCommand>

{
    public async Task<Result> Handle(DeleteAttendancesCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.Attendances
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}