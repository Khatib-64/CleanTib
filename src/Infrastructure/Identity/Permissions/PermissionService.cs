//using CleanTib.Application.Common.Caching;
//using CleanTib.Application.Common.Exceptions;
//using CleanTib.Application.Common.Extensions;
//using CleanTib.Application.Common.Models;
//using CleanTib.Infrastructure.Identity.Entities;
//using CleanTib.Infrastructure.Persistence.Context;
//using CleanTib.Shared.Authorization;
//using CleanTib.Shared.Authorizationz;
//using MediatR;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Localization;

//namespace CleanTib.Infrastructure.Identity.Permissions;

//public class PermissionService : IPermissionService
//{
//    private readonly RoleManager<ApplicationRole> _roleManager;
//    private readonly ApplicationDbContext _context;
//    private readonly IStringLocalizer<PermissionService> _localizer;
//    private readonly ICacheService _cache;
//    private readonly ICacheKeyService _cacheKeys;
//    private readonly IMediator _mediator;

//    public PermissionService(RoleManager<ApplicationRole> roleManager,
//                             ApplicationDbContext context,
//                             IStringLocalizer<PermissionService> localizer,
//                             ICacheService cache,
//                             ICacheKeyService cacheKeys,
//                             IMediator mediator)
//    {
//        _roleManager = roleManager;
//        _context = context;
//        _localizer = localizer;
//        _cache = cache;
//        _cacheKeys = cacheKeys;
//        _mediator = mediator;
//    }

//    private async Task<List<PermissionDto>> PermissionsList(IReadOnlyList<SvuPermission> permissions, string? roleId = null)
//    {
//        var result = new List<PermissionDto>();
//        foreach (var permission in permissions)
//        {
//            result.Add(new PermissionDto() { Permission = permission.Name, Description = permission.Description });
//        }

//        if (!string.IsNullOrEmpty(roleId))
//        {
//            var rolePermissions = await _cache.GetOrSetAsync(
//               _cacheKeys.GetCacheKey(CTClaims.RolePermission, roleId),
//               async () =>
//               {
//                   var permissions = await _context.RoleClaims.Where(a => a.RoleId == roleId && a.ClaimType == CTClaims.Permission).ToListAsync();
//                   return permissions;
//               });

//            if (rolePermissions != null)
//            {
//                var permissionValues = rolePermissions.Select(x => x.ClaimValue);
//                foreach (var permission in result)
//                {
//                    permission.IsSelected = permissionValues.Contains(permission.Permission);
//                }
//            }
//        }

//        return result;
//    }

//    public async Task<Result<List<PermissionDto>>> GetPermissionsListAsync(string? module = null, string? roleId = null)
//    {
//        List<PermissionDto>? result = null;
//        if (!string.IsNullOrEmpty(roleId))
//        {
//            var role = await _roleManager.Roles.SingleOrDefaultAsync(x => x.Id == roleId);
//            if (role is null)
//            {
//                throw new NotFoundException("Role Not Found");
//            }

//            result = await PermissionsList(Shared.Authorizationz.CTPermissions.AllPermissions.Where(x => module.HasValue() ? x.Module == module : true).ToList(), roleId);

//            if (module.HasValue())
//                result = result.OrderBy(x => Shared.Authorizationz.CTResource.GetModuleTableOrder(module, x.Permission.Split('.')[2])).ToList();
//        }

//        return await Result<List<PermissionDto>>.SuccessAsync(result);
//    }

//    public async Task<Result<List<TablePermissionDto>>> GetPermissionsTabledListAsync(string? module = null, string? roleId = null)
//    {
//        List<PermissionDto>? result = null;
//        if (!string.IsNullOrEmpty(roleId))
//        {
//            var role = await _roleManager.Roles.SingleOrDefaultAsync(x => x.Id == roleId);
//            if (role is null)
//            {
//                throw new NotFoundException("Role Not Found");
//            }

//            result = await PermissionsList(Shared.Authorizationz.CTPermissions.AllPermissions.Where(x => module.HasValue() ? x.Module == module : true).ToList(), roleId);

//            if (module.HasValue())
//                result = result.OrderBy(x => CTResource.OrderTable(module, x.Permission.Split('.')[2])).ToList();
//        }

//        result ??= new List<PermissionDto>();

//        var tabledPermissions = from item in result
//                                group item by item.Permission!.Split('.')[2] into tabled
//                                select new TablePermissionDto() { Table = tabled.Key, Permissions = tabled.ToList(), };
//        return await Result<List<TablePermissionDto>>.SuccessAsync(tabledPermissions.ToList());
//    }

//    public async Task<Result<List<SchemaPermissionDto>>> GetPermissionsSchemaListAsync(string? module = null, string? roleId = null)
//    {
//        List<PermissionDto>? result = null;
//        if (!string.IsNullOrEmpty(roleId))
//        {
//            var role = await _roleManager.Roles.SingleOrDefaultAsync(x => x.Id == roleId);
//            if (role is null)
//            {
//                throw new NotFoundException("Role Not Found");
//            }

//            result = await PermissionsList(Shared.Authorizationz.CTPermissions.AllPermissions.Where(x => module.HasValue() ? x.Module == module : true).ToList(), roleId);

//            if (module.HasValue())
//                result = result.OrderBy(x => CTResource.OrderTable(module, x.Permission.Split('.')[2])).ToList();
//        }

//        result ??= [];

//        var schemaPermissions = from item in result
//                                group item by item.Permission!.Split('.')[1] into tabled
//                                select new SchemaPermissionDto()
//                                {
//                                    Schema = tabled.Key,
//                                    Tables = (from table in tabled.ToList()
//                                              group table by table.Permission!.Split('.')[2] into x
//                                              select new TablePermissionDto() { Table = x.Key, Permissions = x.ToList() }).ToList()
//                                };

//        return await Result<List<SchemaPermissionDto>>.SuccessAsync(schemaPermissions.ToList());
//    }

//    public async Task<Result<List<TablePermissionDto>>> GetUserPermissionsTabledListAsync(string? module = null, string? userId = null)
//    {
//        List<PermissionDto>? allPermissions = await PermissionsList(Shared.Authorizationz.CTPermissions.AllPermissions.Where(x => module.HasValue() ? x.Module == module : true).ToList());

//        var userPermissions = (await _mediator.Send(new GetUserPermissionsRequest { UserId = userId })).Distinct().Where(x => allPermissions.Any(y => y.Permission == x));

//        foreach (var item in allPermissions.Where(x => userPermissions.Contains(x.Permission)))
//        {
//            item.IsSelected = true;
//        }

//        var tabledPermissions = from item in allPermissions
//                                group item by item.Permission!.Split('.')[2] into tabled
//                                select new TablePermissionDto() { Table = tabled.Key, Permissions = tabled.ToList(), };

//        return await Result<List<TablePermissionDto>>.SuccessAsync(tabledPermissions.ToList());
//    }

//    public async Task<Result<List<SchemaPermissionDto>>> GetUserPermissionsSchemaListAsync(string userId)
//    {
//        var allPermissions = (await PermissionsList(Shared.Authorizationz.CTPermissions.AllPermissions.ToList())).DistinctBy(x => x.Permission);

//        var userPermissions = (await _mediator.Send(new GetUserPermissionsRequest { UserId = userId })).Distinct().Where(x => allPermissions.Any(y => y.Permission == x));

//        foreach (var item in allPermissions.Where(x => userPermissions.Contains(x.Permission)))
//        {
//            item.IsSelected = true;
//        }

//        var schemaPermissions = from item in allPermissions
//                                group item by item.Permission!.Split('.')[1] into tabled
//                                select new SchemaPermissionDto()
//                                {
//                                    Schema = tabled.Key,
//                                    Tables = (from table in tabled.ToList()
//                                              group table by table.Permission!.Split('.')[2] into x
//                                              select new TablePermissionDto() { Table = x.Key, Permissions = x.ToList() }).ToList()
//                                };


//        return await Result<List<SchemaPermissionDto>>.SuccessAsync(schemaPermissions.ToList());
//    }
//}
