﻿using EasyPOS.Application.Common.Abstractions.Identity;

namespace EasyPOS.Application.Features.Admin.AppUsers.Commands;

public record DeleteAppUserCommand(string Id) : ICacheInvalidatorCommand
{
    public string CacheKey => CacheKeys.AppUser;
}

internal sealed class DeleteAppUserCommandHandler(
    IIdentityService identityService)
    : ICommandHandler<DeleteAppUserCommand>
{
    public async Task<Result> Handle(DeleteAppUserCommand request, CancellationToken cancellationToken)
    {
        return await identityService.DeleteUserAsync(request.Id, cancellationToken);

    }
}