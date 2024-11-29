//using CleanTib.Application.Common.Interfaces;
//using CleanTib.Application.Common.Models;

//namespace CleanTib.Infrastructure.Identity.Permissions;
//public interface IPermissionService : ITransientService
//{
//    Task<Result<List<PermissionDto>>> GetPermissionsListAsync(string? module = null, string? roleId = null);
//    Task<Result<List<TablePermissionDto>>> GetPermissionsTabledListAsync(string? module = null, string? roleId = null);
//    Task<Result<List<SchemaPermissionDto>>> GetPermissionsSchemaListAsync(string? module = null, string? roleId = null);
//    Task<Result<List<TablePermissionDto>>> GetUserPermissionsTabledListAsync(string? module = null, string? userId = null);
//    Task<Result<List<SchemaPermissionDto>>> GetUserPermissionsSchemaListAsync(string userId);
//}