using Microsoft.AspNetCore.Authorization;

namespace CleanTib.Infrastructure.Auth.Permissions;

internal class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; private set; }

    public PermissionRequirement(string permission)
        => Permission = permission;
}