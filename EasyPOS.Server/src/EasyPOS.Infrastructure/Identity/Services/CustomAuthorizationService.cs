using EasyPOS.Application.Common.Abstractions.Identity;
using EasyPOS.Application.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EasyPOS.Infrastructure.Identity.Services;

public class CustomAuthorizationService(
    UserManager<ApplicationUser> userManager,
    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService,
    ILogger<CustomAuthorizationService> logger) : ICustomAuthorizationService
{
    public async Task<Result> AuthorizeAsync(string userId, string policyName, CancellationToken cancellation = default)
    {
        try
        {
            var user = await userManager.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == userId, cancellation);

            if (user is null)
            {
                logger.LogWarning("User not found for authorization. UserId: {UserId}", userId);
                return Result.Failure(Error.NotFound(nameof(user), ErrorMessages.USER_NOT_FOUND));
            }

            var principal = await userClaimsPrincipalFactory.CreateAsync(user);

            var result = await authorizationService.AuthorizeAsync(principal, policyName);

            return result.Succeeded
                ? Result.Success()
                : Result.Failure(Error.Unauthorized(nameof(ErrorType.Unauthorized), "User is not authorized for the requested policy."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during authorization for UserId: {UserId}, Policy: {PolicyName}", userId, policyName);
            return Result.Failure(Error.Failure(ErrorMessages.InvalidOperation, "An internal error occurred during authorization."));
        }
    }

    public async Task<Result> IsInRoleAsync(string userId, string role, CancellationToken cancellation = default)
    {
        var user = await userManager.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId, cancellation);

        if (user is null) return Result.Failure(Error.NotFound(nameof(user), ErrorMessages.USER_NOT_FOUND));

        return await userManager.IsInRoleAsync(user, role)
            ? Result.Success()
            : Result.Failure(Error.Forbidden(nameof(ErrorType.Forbidden), "You have no permission to access the resource"));
    }
}


