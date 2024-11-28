using EasyPOS.Application.Common.Abstractions.Identity;

namespace EasyPOS.Application.Features.Admin.Roles.Commands;

public record DeleteRoleCommand(string Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.Role;
}

internal sealed class DeleteRoleCommandHandler(
    IIdentityRoleService roleService)
    : ICommandHandler<DeleteRoleCommand>
{
    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        return await roleService.DeleteRoleAsync(request.Id, cancellationToken);

    }
}
