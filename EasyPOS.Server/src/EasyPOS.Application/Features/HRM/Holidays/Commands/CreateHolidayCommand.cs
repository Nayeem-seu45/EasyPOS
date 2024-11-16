using EasyPOS.Domain.HRM;

namespace EasyPOS.Application.Features.HRM.Holidays.Commands;

public record CreateHolidayCommand(
    string? Title, 
    string? Description, 
    bool IsActive
    ): ICacheInvalidatorCommand<Guid>
{
    public string CacheKey => CacheKeys.Holiday;
}
    
internal sealed class CreateHolidayCommandHandler(
    IApplicationDbContext dbContext) 
    : ICommandHandler<CreateHolidayCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateHolidayCommand request, CancellationToken cancellationToken)
    {
       var entity = request.Adapt<Holiday>();

       dbContext.Holidays.Add(entity);

       await dbContext.SaveChangesAsync(cancellationToken);

       return  entity.Id;
    }
}