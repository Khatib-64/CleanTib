using System.Collections.ObjectModel;

namespace CleanTib.Shared.Authorizationz;

public record CTPermission(string Description, string Action, string Resource, string Module, bool isRoot = false)
{
    public string Name => NameFor(Action, Resource, Module);
    public static string NameFor(string action, string resource, string? module = null)
        => string.IsNullOrWhiteSpace(module) ?
        $"Permissions.{CTResource.DetermineModuleForTable(resource)}.{resource}.{action}" :
        $"Permissions.{module}.{resource}.{action}";
}

public static class CTModule
{
    public const string UsersManagement = nameof(UsersManagement);
    public const string Default = nameof(Default);
}

public static class CTAction
{
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Search = nameof(Search);
    public const string Details = nameof(Details);
    public const string QuickLookup = nameof(QuickLookup);
    public const string Delete = nameof(Delete);
    public const string Activate = nameof(Activate);
    public const string Export = nameof(Export);
    public const string Generate = nameof(Generate);
}

public static class CTCustomActions
{
    public const string ResetUserPassword = nameof(ResetUserPassword);
    public const string AssignUserToRole = nameof(AssignUserToRole);
    public const string UnassignUserFromRole = nameof(UnassignUserFromRole);
    public const string SaveChanges = nameof(SaveChanges);
    public const string Cancel = nameof(Cancel);
    public const string Unlock = nameof(Unlock);
    public const string Upload = nameof(Upload);
    public const string Download = nameof(Download);
}

public static class CTResource
{
    #region UsersManagement
    public const string Users = nameof(Users);
    public const string UserRoles = nameof(UserRoles);
    public const string Roles = nameof(Roles);
    public const string RoleClaims = nameof(RoleClaims);

    public static int UsersManagementTableOrder(string? table) => table switch
    {
        Users => 1,
        Roles => 2,
        UserRoles => 3,
        RoleClaims => 4,
        _ => 0
    };
    #endregion

    public static int GetModuleTableOrder(string? module, string? table) => module switch
    {
        CTModule.UsersManagement => UsersManagementTableOrder(table),
        _ => -1
    };

    public static string DetermineModuleForTable(string? table)
    {
        if (UsersManagementTableOrder(table) is not 0)
            return CTModule.UsersManagement;

        return CTModule.Default;
    }
}

public static class CTPermissions
{
    public static CTPermission[] GroupPermissions(
        string resource,
        string module,
        bool withCreate = true,
        bool withDetails = true,
        bool withSearch = true,
        bool withUpdate = true,
        bool withActivate = true,
        bool withDelete = true,
        bool withUpload = false,
        bool withDownload = false,
        bool withQuickLookUp = false)
    {
        List<CTPermission> permissions = [];

        if (withCreate)
            permissions.Add(new($"Create {resource}", CTAction.Create, resource, module));

        if (withUpdate)
            permissions.Add(new($"Update {resource}", CTAction.Update, resource, module));

        if (withSearch)
            permissions.Add(new($"Search {resource}", CTAction.Search, resource, module));

        if (withDetails)
            permissions.Add(new($"Details {resource}", CTAction.Details, resource, module));

        if (withDelete)
            permissions.Add(new($"Delete {resource}", CTAction.Delete, resource, module));

        if (withActivate)
            permissions.Add(new($"Activate {resource}", CTAction.Activate, resource, module));

        if (withUpload)
            permissions.Add(new($"Upload {resource}", CTCustomActions.Upload, resource, module));

        if (withDownload)
            permissions.Add(new($"Download {resource}", CTCustomActions.Download, resource, module));

        if (withQuickLookUp)
            permissions.Add(new($"QuickLookUp {resource}", CTAction.QuickLookup, resource, module));

        return permissions.ToArray();
    }

    public static CTPermission[] GroupWithPermission(string resource, string module, string action)
        => [new($"{action} {resource}", action, resource, module)];

    private static readonly CTPermission[] UsersManagement =
    Array.Empty<CTPermission>()
    .Concat(GroupPermissions(CTResource.Users, CTModule.UsersManagement, withQuickLookUp: true)
        .Concat(GroupWithPermission(CTResource.Users, CTModule.UsersManagement, CTCustomActions.Unlock))
        .Concat(GroupWithPermission(CTResource.Users, CTModule.UsersManagement, CTCustomActions.ResetUserPassword)))
        .Concat(GroupWithPermission(CTResource.UserRoles, CTModule.UsersManagement, CTCustomActions.AssignUserToRole)
        .Concat(GroupWithPermission(CTResource.UserRoles, CTModule.UsersManagement, CTCustomActions.UnassignUserFromRole))
    .Concat(GroupPermissions(CTResource.UserRoles, CTModule.UsersManagement, false, false, false, false, false))
        .Concat(GroupWithPermission(CTResource.UserRoles, CTModule.UsersManagement, CTCustomActions.SaveChanges)))
    .Concat(GroupPermissions(CTResource.Roles, CTModule.UsersManagement, withQuickLookUp: true))
    .ToArray();

    public static IReadOnlyList<CTPermission> Admin { get; } = new ReadOnlyCollection<CTPermission>(AllPermissions!);

    public static CTPermission[] AllPermissions { get; } =
    Array.Empty<CTPermission>()
    .Concat(UsersManagement)
    .ToArray();
}
