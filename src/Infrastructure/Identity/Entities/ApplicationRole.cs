using Microsoft.AspNetCore.Identity;

namespace CleanTib.Infrastructure.Identity.Entities;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }

    public ApplicationRole(string name, string? description = null)
        : base(name)
    {
        Description = description;
        NormalizedName = name.ToUpperInvariant();
    }
}