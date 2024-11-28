using EasyPOS.Application.Common.Abstractions.Identity;

namespace EasyPOS.Application.Features.Admin.AppUsers.Commands;

public record AddToRolesCommand(
     string Id,
     List<string> RoleNames
    ) : ICacheInvalidatorCommand
{
    [JsonIgnore]
    public string CacheKey => CacheKeys.AppUser;
}

internal sealed class AddToRolesCommandHandler(IIdentityService identityService)
    : ICommandHandler<AddToRolesCommand>
{
    public async Task<Result> Handle(AddToRolesCommand request, CancellationToken cancellationToken)
    {
        return await identityService.AddToRolesAsync(request.Id, request.RoleNames, cancellationToken);
    }
}
