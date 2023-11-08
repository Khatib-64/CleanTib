using Microsoft.AspNetCore.Identity;

namespace CleanTib.Infrastructure.Identity.Entities;

public class RoleClaim : IdentityRoleClaim<string>
{
    public string? CreatedBy { get; init; }
    public DateTime CreatedOn { get; init; }
}