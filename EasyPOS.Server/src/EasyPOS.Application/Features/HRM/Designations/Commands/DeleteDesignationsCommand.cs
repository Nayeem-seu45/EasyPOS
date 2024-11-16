namespace EasyPOS.Application.Features.HRM.Designations.Commands;

public record DeleteDesignationsCommand(Guid[] Ids): ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Designation;
}

internal sealed class DeleteDesignationsCommandHandler(
    ISqlConnectionFactory sqlConnection) 
    : ICommandHandler<DeleteDesignationsCommand>

{
    public async Task<Result> Handle(DeleteDesignationsCommand request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            Delete dbo.Designations
            WHERE Id IN @Id
            """;

        await connection.ExecuteAsync(sql, new { request.Ids });

        return Result.Success();
    }

}