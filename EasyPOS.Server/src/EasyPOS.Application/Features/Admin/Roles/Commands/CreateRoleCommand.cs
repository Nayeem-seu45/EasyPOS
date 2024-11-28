using EasyPOS.Application.Common.Abstractions.Identity;

namespace EasyPOS.Application.Features.Admin.Roles.Commands;

public record CreateRoleCommand(
     string Name,
     List<Guid> Rolemenus,
     List<string> Permissions
    ) : ICacheInvalidatorCommand<string>
{
    [JsonIgnore]
    public string CacheKey => CacheKeys.Role;
}

internal sealed class CreateRoleCommandHandler(IIdentityRoleService roleService) 
    : ICommandHandler<CreateRoleCommand, string>
{
    public async Task<Result<string>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        return await roleService.CreateRoleAsync(request.Name, request.Rolemenus, request.Permissions, cancellationToken);
    }
}


