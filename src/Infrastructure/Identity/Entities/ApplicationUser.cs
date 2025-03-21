using CleanTib.Domain.Attributes;
using Microsoft.AspNetCore.Identity;

namespace CleanTib.Infrastructure.Identity.Entities;

[ClassSupportDeepSearch(1)]
public class ApplicationUser : IdentityUser
{
    [ColumnSupportDeepSearch]
    public string? FirstName { get; set; }
    [ColumnSupportDeepSearch]
    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public string? ObjectId { get; set; }
}