namespace EasyPOS.Application.Features.HRM.LeaveRequests.Commands;

public record DeleteLeaveRequestsCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.LeaveRequest;
}

internal sealed class DeleteLeaveRequestsCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteLeaveRequestsCommand>

{
    public async Task<Result> Handle(DeleteLeaveRequestsCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.LeaveRequests
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}
