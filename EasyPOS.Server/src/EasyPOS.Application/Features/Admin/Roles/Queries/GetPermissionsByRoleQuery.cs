using EasyPOS.Application.Common.Abstractions.Identity;

namespace EasyPOS.Application.Features.Admin.Roles.Queries;

public record GetPermissionsByRoleQuery(string RoleId)
    : ICacheableQuery<IList<DynamicTreeNodeModel>>
{
    [JsonIgnore]
    public string CacheKey => $"Role_{RoleId}_Permissions";

    public bool? AllowCache => false;

    public TimeSpan? Expiration => null;
}

internal sealed class GetPermissionsByRoleQueryHandler(IIdentityRoleService roleService)
    : IQueryHandler<GetPermissionsByRoleQuery, IList<DynamicTreeNodeModel>>
{
    public async Task<Result<IList<DynamicTreeNodeModel>>> Handle(GetPermissionsByRoleQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(roleService.GetAllPermissions());
    }
}
