﻿using System.Transactions;
using EasyPOS.Application.Common.Abstractions;
using EasyPOS.Application.Common.Abstractions.Identity;
using EasyPOS.Application.Common.Constants;
using EasyPOS.Application.Common.Models;
using EasyPOS.Application.Features.Admin.Roles.Queries;
using EasyPOS.Domain.Admin;
using EasyPOS.Infrastructure.Identity.Permissions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EasyPOS.Infrastructure.Identity.Services;

internal class IdentityRoleService(
    RoleManager<IdentityRole> roleManager,
    IdentityContext identityContext,
    IApplicationDbContext appDbContext,
    ILogger<IdentityRoleService> logger)
    : IIdentityRoleService
{
    public async Task<Result<string>> CreateRoleAsync(
        string name,
        List<Guid> rolemenus,
        List<string> permissions,
        CancellationToken cancellation = default)
    {
        using var transaction = await identityContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var role = new IdentityRole
            {
                Name = name,
                NormalizedName = name.ToUpper()
            };

            await identityContext.Roles.AddAsync(role, cancellation);
            await identityContext.SaveChangesAsync(cancellation);

            foreach (var appmenuId in rolemenus ?? [])
            {
                appDbContext.RoleMenus.Add(new RoleMenu
                {
                    RoleId = role.Id,
                    AppMenuId = appmenuId,
                });
            }

            foreach (var permission in permissions)
            {
                identityContext.RoleClaims.Add(new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = CustomClaimTypes.Permission,
                    ClaimValue = permission
                });
            }

            await appDbContext.SaveChangesAsync(cancellation);
            await identityContext.SaveChangesAsync(cancellation);

            await transaction.CommitAsync(cancellation);

            return Result.Success(role.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            logger.LogError(ex, "Error occured to save role");
            return Result.Failure<string>(Error.Failure("Role.Create", "Error occured to save role."));
        }
    }

    public async Task<Result> UpdateRoleAsync(
        string id,
        string name,
        List<Guid> rolemenus,
        List<string> permissions,
        CancellationToken cancellation = default)
    {
        using var transaction = await identityContext.Database.BeginTransactionAsync(cancellation);

        try
        {
            var role = await identityContext.Roles.FindAsync(id, cancellation);

            if (role is null)
                Result.Failure(Error.Failure("Role.Update", ErrorMessages.ROLE_NOT_FOUND));

            role!.Name = name;
            role!.NormalizedName = name.ToUpper();

            await RemoveAndAddPermissionAsync(role!, permissions, cancellation);
            await RemoveAndAddRoleMenuAsync(role!, rolemenus, cancellation);

            await identityContext.SaveChangesAsync(cancellation);
            await appDbContext.SaveChangesAsync(cancellation);

            await transaction.CommitAsync(cancellation);

            return Result.Success();

        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            logger.LogError(ex, "Error occured to update role");
            return Result.Failure<string>(Error.Failure("Role.Create", "Error occured to update role."));

        }
    }

    public async Task<Result> DeleteRoleAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        // Use using statement to ensure proper disposal
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            // Find the role
            var role = await roleManager.FindByIdAsync(id);
            if (role is null)
                return Result.Failure(Error.Failure("Role.Delete", ErrorMessages.ROLE_NOT_FOUND));

            // Remove all claims associated with this role first
            var roleClaims = await roleManager.GetClaimsAsync(role);
            foreach (var claim in roleClaims)
            {
                var removeClaimResult = await roleManager.RemoveClaimAsync(role, claim);
                if (!removeClaimResult.Succeeded)
                {
                    return Result.Failure(Error.Failure("Role.Delete",
                        $"Failed to remove role claim: {claim.Type}"));
                }
            }

            // Delete the role
            var deleteResult = await roleManager.DeleteAsync(role);
            if (!deleteResult.Succeeded)
            {
                return Result.Failure(Error.Failure("Role.Delete", ErrorMessages.UNABLE_DELETE_ROLE));
            }

            // Complete the transaction
            transaction.Complete();
            return Result.Success();
        }
        catch (Exception ex)
        {
            // Log the exception if you have a logger
            // _logger.LogError(ex, "Error deleting role with ID {RoleId}", id);

            return Result.Failure(Error.Failure("Role.Delete",
                "An unexpected error occurred while deleting the role"));
        }
    }

    public async Task<Result<RoleModel>> GetRoleAsync(
        string id,
        CancellationToken cancellation = default)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role is null)
            Result.Failure<RoleModel>(Error.Failure("Role.Delete", ErrorMessages.ROLE_NOT_FOUND));

        var permissions = await roleManager.GetClaimsAsync(role);

        var roleMenus = await appDbContext.RoleMenus
            .AsNoTracking()
            .Where(x => x.RoleId.ToLower() == id.ToLower())
            .Select(x => x.AppMenuId)
            .ToListAsync(cancellation);

        return Result.Success(new RoleModel
        {
            Id = role!.Id,
            Name = role.Name!,
            RoleMenus = roleMenus,
            Permissions = permissions?.Select(x => x.Value).ToList()
        });
    }

    public async Task<Result> AddOrRemoveClaimsToRoleAsync(
        string roleId,
        List<string> permissions,
        CancellationToken cancellation = default)
    {
        var role = await roleManager.FindByIdAsync(roleId);

        if (role is null)
            Result.Failure(Error.Failure("Role.Update", ErrorMessages.ROLE_NOT_FOUND));

        var result = await RemoveAndAddPermissionAsync(role!, permissions, cancellation);

        await identityContext.SaveChangesAsync(cancellation);

        return result ? Result.Success() : Result.Failure(Error.Failure("Role.Permission", ErrorMessages.UNABLE_UPDATE_PERMISSION));
    }

    public Result<IList<DynamicTreeNodeModel>> GetAllPermissions()
    {
        return Result.Success(PermissionHelper.MapPermissionsToTree());
    }

    private async Task<bool> RemoveAndAddPermissionAsync(
        IdentityRole role,
        List<string> permissions,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var existedPermissions = await identityContext.RoleClaims
            .Where(x => x.RoleId == role.Id)
            .ToListAsync(cancellationToken);

            identityContext.RoleClaims.RemoveRange(existedPermissions);

            var newPermissions = permissions.Select(x => new IdentityRoleClaim<string>
            {
                RoleId = role.Id,
                ClaimType = CustomClaimTypes.Permission,
                ClaimValue = x
            });

            identityContext.RoleClaims.AddRange(newPermissions);

            return true;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fail to update role permission");
            return true;
        }
    }

    private async Task<bool> RemoveAndAddRoleMenuAsync(
        IdentityRole role,
        List<Guid> rolemenus,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var existedRoleMenus = await appDbContext.RoleMenus
            .Where(x => x.RoleId == role.Id)
            .ToListAsync(cancellationToken);

            appDbContext.RoleMenus.RemoveRange(existedRoleMenus);

            foreach (var appmenuId in rolemenus ?? [])
            {
                appDbContext.RoleMenus.Add(new RoleMenu
                {
                    RoleId = role.Id,
                    AppMenuId = appmenuId,
                });
            }
            return true;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fail to update RoleMenu");
            return true;
        }
    }
}
