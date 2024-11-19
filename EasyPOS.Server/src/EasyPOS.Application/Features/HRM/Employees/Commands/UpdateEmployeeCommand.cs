namespace EasyPOS.Application.Features.HRM.Employees.Commands;

public record UpdateEmployeeCommand(
    Guid Id,
    string? Code,
    string FirstName,
    string? LastName,
    string? Gender,
    string? NID,
    DateOnly? DOB,
    Guid? WarehouseId,
    Guid? DepartmentId,
    Guid? DesignationId,
    Guid? WorkingShiftId,
    Guid? ReportTo,
    string? Email,
    string? PhoneNo,
    string? MobileNo,
    string? Country,
    string? City,
    string? Address
    ) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Employee;
}

internal sealed class UpdateEmployeeCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<UpdateEmployeeCommand>
{
    public async Task<Result> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Employees.FindAsync([request.Id], cancellationToken);

        if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        request.Adapt(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
