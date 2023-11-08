using CleanTib.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanTib.Infrastructure.Persistence.Configuration;

public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .ToTable("Users", SchemaNames.Identity);

        builder
            .Property(u => u.ObjectId)
                .HasMaxLength(256);
    }
}

public class ApplicationRoleConfig : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder) =>
        builder
            .ToTable("Roles", SchemaNames.Identity);
}

public class RoleClaimConfig : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder) =>
        builder
            .ToTable("RoleClaims", SchemaNames.Identity);
}

public class IdentityUserRoleConfig : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder) =>
        builder
            .ToTable("UserRoles", SchemaNames.Identity);
}

public class IdentityUserClaimConfig : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder) =>
        builder
            .ToTable("UserClaims", SchemaNames.Identity);
}

public class IdentityUserLoginConfig : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder) =>
        builder
            .ToTable("UserLogins", SchemaNames.Identity);
}

public class IdentityUserTokenConfig : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder) =>
        builder
            .ToTable("UserTokens", SchemaNames.Identity);
}