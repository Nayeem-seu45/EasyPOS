using System.Text.Json.Serialization;
using EasyPOS.Application.Common.Abstractions.Identity;
using EasyPOS.Application.Common.Abstractions.Messaging;
using EasyPOS.Application.Common.Models;
using EasyPOS.Domain.Shared;

namespace EasyPOS.Application.Features.Admin.Roles.Queries;

public record GetPermissionTreeSelectListQuery
    : ICacheableQuery<List<DynamicTreeNodeModel>>
{
    [JsonIgnore]
    public string CacheKey => $"Role_Permissions";

    public bool? AllowCache => false;

    public TimeSpan? Expiration => null;
}

internal sealed class GetPermissionNodeListQueryHandler(IIdentityRoleService roleService)
    : IQueryHandler<GetPermissionTreeSelectListQuery, List<DynamicTreeNodeModel>>
{
    public async Task<Result<List<DynamicTreeNodeModel>>> Handle(GetPermissionTreeSelectListQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(roleService.GetAllPermissions().Value.ToList());
    }
}
