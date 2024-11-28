﻿using EasyPOS.Application.Common.Abstractions.Identity;

namespace EasyPOS.Application.Features.Admin.AppUsers.Commands;

public record UpdateAppUserCommand(
     string Id,
     string Username,
     string Email,
     string FirstName,
     string LastName,
     string PhoneNumber,
     bool IsActive,
     List<string>? Roles
    ) : ICacheInvalidatorCommand
{
    [JsonIgnore]
    public string CacheKey => CacheKeys.AppUser;
}

internal sealed class UpdateAppUserCommandHandler(IIdentityService identityService)
    : ICommandHandler<UpdateAppUserCommand>
{
    public async Task<Result> Handle(UpdateAppUserCommand request, CancellationToken cancellationToken)
    {
        return await identityService.UpdateUserAsync(request, cancellationToken);
    }
}
