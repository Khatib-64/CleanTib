using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanTib.Infrastructure.Persistence.Initialization;

internal class DatabaseInitializer : IDatabaseInitializer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task InitializeDatabasesAsync(CancellationToken cancellationToken)
    {
        await InitializeApplicationDbAsync(cancellationToken);
    }

    public async Task InitializeApplicationDbAsync(CancellationToken cancellationToken)
    {
        // First create a new scope
        using var scope = _serviceProvider.CreateScope();

        // Then set current tenant so the right connectionstring is used

        // Then run the initialization in the new scope
        await scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>()
            .InitializeAsync(cancellationToken);

        _logger.LogInformation("Database Initialized Successfully");
    }

    // private async Task InitializeTenantDbAsync(CancellationToken cancellationToken)
    // {
    //    if (_tenantDbContext.Database.GetPendingMigrations().Any())
    //    {
    //        _logger.LogInformation("Applying Root Migrations.");
    //        await _tenantDbContext.Database.MigrateAsync(cancellationToken);
    //    }

    // await SeedRootTenantAsync(cancellationToken);
    // }

    // private async Task SeedRootTenantAsync(CancellationToken cancellationToken)
    // {
    //    if (await _tenantDbContext.TenantInfo.FindAsync(new object?[] { MultitenancyConstants.Root.Id }, cancellationToken: cancellationToken) is null)
    //    {
    //        var rootTenant = new FSHTenantInfo(
    //            MultitenancyConstants.Root.Id,
    //            MultitenancyConstants.Root.Name,
    //            string.Empty,
    //            MultitenancyConstants.Root.EmailAddress);

    // rootTenant.SetValidity(DateTime.UtcNow.AddYears(1));

    // _tenantDbContext.TenantInfo.Add(rootTenant);

    // await _tenantDbContext.SaveChangesAsync(cancellationToken);
    //    }
    // }
}
