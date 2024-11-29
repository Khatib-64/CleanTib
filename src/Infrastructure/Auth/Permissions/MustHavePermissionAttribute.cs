using CleanTib.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace CleanTib.Infrastructure.Auth.Permissions;

public class MustHavePermissionAttribute : AuthorizeAttribute
{
    public MustHavePermissionAttribute(string action, string resource) =>
        Policy = CTPermission.NameFor(action, resource);
}