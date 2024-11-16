namespace EasyPOS.Application.Features.HRM.LeaveTypes.Commands;

public record DeleteLeaveTypesCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.LeaveType;
}

internal sealed class DeleteLeaveTypesCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteLeaveTypesCommand>

{
    public async Task<Result> Handle(DeleteLeaveTypesCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.LeaveTypes
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}