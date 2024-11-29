using System.Collections.ObjectModel;

namespace CleanTib.Shared.Authorization;

public static class CTAction
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
    public const string Generate = nameof(Generate);
    public const string Clean = nameof(Clean);
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
}

public static class CTResource
{
    public const string Tenants = nameof(Tenants);
    public const string Dashboard = nameof(Dashboard);
    public const string Hangfire = nameof(Hangfire);
    public const string Users = nameof(Users);
    public const string UserRoles = nameof(UserRoles);
    public const string Roles = nameof(Roles);
    public const string RoleClaims = nameof(RoleClaims);
    public const string Products = nameof(Products);
    public const string Brands = nameof(Brands);
}

public static class CTPermissions
{
    private static readonly CTPermission[] _all =
    [
        new CTPermission("View Dashboard", CTAction.View, CTResource.Dashboard),
        new("View Hangfire", CTAction.View, CTResource.Hangfire),
        new("View Users", CTAction.View, CTResource.Users),
        new("Search Users", CTAction.Search, CTResource.Users),
        new("Create Users", CTAction.Create, CTResource.Users),
        new("Update Users", CTAction.Update, CTResource.Users),
        new("Delete Users", CTAction.Delete, CTResource.Users),
        new("Export Users", CTAction.Export, CTResource.Users),
        new("View UserRoles", CTAction.View, CTResource.UserRoles),
        new("Update UserRoles", CTAction.Update, CTResource.UserRoles),
        new("View Roles", CTAction.View, CTResource.Roles),
        new("Create Roles", CTAction.Create, CTResource.Roles),
        new("Update Roles", CTAction.Update, CTResource.Roles),
        new("Delete Roles", CTAction.Delete, CTResource.Roles),
        new("View RoleClaims", CTAction.View, CTResource.RoleClaims),
        new("Update RoleClaims", CTAction.Update, CTResource.RoleClaims),
        new("View Products", CTAction.View, CTResource.Products, IsBasic: true),
        new("Search Products", CTAction.Search, CTResource.Products, IsBasic: true),
        new("Create Products", CTAction.Create, CTResource.Products),
        new("Update Products", CTAction.Update, CTResource.Products),
        new("Delete Products", CTAction.Delete, CTResource.Products),
        new("Export Products", CTAction.Export, CTResource.Products),
        new("View Brands", CTAction.View, CTResource.Brands, IsBasic: true),
        new("Search Brands", CTAction.Search, CTResource.Brands, IsBasic: true),
        new("Create Brands", CTAction.Create, CTResource.Brands),
        new("Update Brands", CTAction.Update, CTResource.Brands),
        new("Delete Brands", CTAction.Delete, CTResource.Brands),
        new("Generate Brands", CTAction.Generate, CTResource.Brands),
        new("Clean Brands", CTAction.Clean, CTResource.Brands),
        new("View Tenants", CTAction.View, CTResource.Tenants, IsRoot: true),
        new("Create Tenants", CTAction.Create, CTResource.Tenants, IsRoot: true),
        new("Update Tenants", CTAction.Update, CTResource.Tenants, IsRoot: true),
        new("Upgrade Tenant Subscription", CTAction.UpgradeSubscription, CTResource.Tenants, IsRoot: true)
    ];

    public static IReadOnlyList<CTPermission> All { get; } = new ReadOnlyCollection<CTPermission>(_all);
    public static IReadOnlyList<CTPermission> Root { get; } = new ReadOnlyCollection<CTPermission>(_all.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<CTPermission> Admin { get; } = new ReadOnlyCollection<CTPermission>(_all.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<CTPermission> Basic { get; } = new ReadOnlyCollection<CTPermission>(_all.Where(p => p.IsBasic).ToArray());
}

public record CTPermission(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource) => $"Permissions.{resource}.{action}";
}
