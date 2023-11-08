using CleanTib.Infrastructure.Identity.Entities;
using CleanTib.Infrastructure.Persistence.Context;
using CleanTib.Shared.Authorization;
using CleanTib.Shared.Multitenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static CleanTib.Shared.Multitenancy.MultitenancyConstants;

namespace CleanTib.Infrastructure.Persistence.Initialization;

internal class ApplicationDbSeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly CustomSeederRunner _seederRunner;
    private readonly ILogger<ApplicationDbSeeder> _logger;

    public ApplicationDbSeeder(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, CustomSeederRunner seederRunner, ILogger<ApplicationDbSeeder> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _seederRunner = seederRunner;
        _logger = logger;
    }

    public async Task SeedDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        await SeedRolesAsync(dbContext);
        await SeedAdminUserAsync();
        await _seederRunner.RunSeedersAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(ApplicationDbContext dbContext)
    {
        foreach (string roleName in FSHRoles.DefaultRoles)
        {
            if (await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
                is not ApplicationRole role)
            {
                // Create the role
                _logger.LogInformation("Seeding {role} Role", roleName);
                role = new ApplicationRole(roleName, $"{roleName} Role");
                await _roleManager.CreateAsync(role);
            }

            // Assign permissions
            if (roleName == FSHRoles.Basic)
            {
                await AssignPermissionsToRoleAsync(dbContext, FSHPermissions.Basic, role);
            }
            else if (roleName == FSHRoles.Admin)
            {
                await AssignPermissionsToRoleAsync(dbContext, FSHPermissions.Admin, role);
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(ApplicationDbContext dbContext, IReadOnlyList<FSHPermission> permissions, ApplicationRole role)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);
        foreach (var permission in permissions)
        {
            if (!currentClaims.Any(c => c.Type == FSHClaims.Permission && c.Value == permission.Name))
            {
                _logger.LogInformation("Seeding {role} Permission '{permission}'", role.Name, permission.Name);
                dbContext.RoleClaims.Add(new RoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = FSHClaims.Permission,
                    ClaimValue = permission.Name,
                    CreatedBy = "ApplicationDbSeeder"
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        if (await _userManager.Users.FirstOrDefaultAsync(u => u.Email == Root.EmailAddress)
            is not ApplicationUser adminUser)
        {
            string adminUserName = $"{Root.Id.Trim()}.{FSHRoles.Admin}".ToLowerInvariant();
            adminUser = new ApplicationUser
            {
                FirstName = Root.Id.Trim().ToLowerInvariant(),
                LastName = FSHRoles.Admin,
                Email = Root.EmailAddress,
                UserName = adminUserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = Root.EmailAddress?.ToUpperInvariant(),
                NormalizedUserName = adminUserName.ToUpperInvariant(),
                IsActive = true
            };

            _logger.LogInformation("Seeding Default Admin User", Root.Id);
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, DefaultPassword);
            await _userManager.CreateAsync(adminUser);
        }

        // Assign role to user
        if (!await _userManager.IsInRoleAsync(adminUser, FSHRoles.Admin))
        {
            _logger.LogInformation("Assigning Admin Role to Admin User", Root.Id);
            await _userManager.AddToRoleAsync(adminUser, FSHRoles.Admin);
        }
    }
}