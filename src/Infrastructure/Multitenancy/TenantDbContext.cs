//using CleanTib.Infrastructure.Persistence.Configuration;
//using Microsoft.EntityFrameworkCore;

//namespace CleanTib.Infrastructure.Multitenancy;

//public class TenantDbContext : EFCoreStoreDbContext<FSHTenantInfo>
//{
//    public TenantDbContext(DbContextOptions<TenantDbContext> options)
//        : base(options)
//    {
//        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
//    }

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        base.OnModelCreating(modelBuilder);

//        modelBuilder.Entity<FSHTenantInfo>().ToTable("Tenants", SchemaNames.MultiTenancy);
//    }
//}

// TODO: Remove this