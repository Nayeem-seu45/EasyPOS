using EasyPOS.Application.Features.Admin.Roles.Queries;

namespace EasyPOS.Application.Common.Abstractions.Identity;

public interface IIdentityRoleService
{
    Task<Result<string>> CreateRoleAsync(string name, List<Guid> rolemenus, List<string> permissions, CancellationToken cancellation = default);
    Task<Result> UpdateRoleAsync(string id, string name, List<Guid> rolemenus, List<string> permissions, CancellationToken cancellation = default);
    Task<Result<RoleModel>> GetRoleAsync(string id, CancellationToken cancellation = default);
    Task<Result> DeleteRoleAsync(string id, CancellationToken cancellation = default);
    Task<Result> AddOrRemoveClaimsToRoleAsync(string roleId, List<string> permissions, CancellationToken cancellation = default);
    Result<IList<DynamicTreeNodeModel>> GetAllPermissions();
}
