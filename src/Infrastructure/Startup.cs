using MediatR;
using System.Reflection;
using System.Runtime.CompilerServices;
using CleanTib.Infrastructure.Auth;
using CleanTib.Infrastructure.BackgroundJobs;
using CleanTib.Infrastructure.Caching;
using CleanTib.Infrastructure.Common;
using CleanTib.Infrastructure.Cors;
using CleanTib.Infrastructure.FileStorage;
using CleanTib.Infrastructure.Localization;
using CleanTib.Infrastructure.Mailing;
using CleanTib.Infrastructure.Mapping;
using CleanTib.Infrastructure.Middleware;
using CleanTib.Infrastructure.Notifications;
using CleanTib.Infrastructure.OpenApi;
using CleanTib.Infrastructure.Persistence;
using CleanTib.Infrastructure.Persistence.Initialization;
using CleanTib.Infrastructure.SecurityHeaders;
using CleanTib.Infrastructure.Validations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CleanTib.Infrastructure.Behaviors;

[assembly: InternalsVisibleTo("Infrastructure.Test")]

namespace CleanTib.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var assembly = Assembly.GetAssembly(typeof(Application.Startup));
        MapsterSettings.Configure();
        return services
            .AddRouting(options => options.LowercaseUrls = true)
            .AddApiVersioning()
            .AddAuth(config)
            .AddCorsPolicy(config)
            .AddMediatR(assembly!)
            .AddRequestLogging(config)
            .AddExceptionMiddleware()
            .AddPersistence()
            .AddCaching(config)
            .AddBackgroundJobs(config)
            .AddHealthCheck()
            .AddPOLocalization(config)
            .AddMailing(config)
            .AddNotifications(config)
            .AddOpenApiDocumentation(config)
            .AddBehaviours()
            .AddServices();
    }

    private static IServiceCollection AddApiVersioning(this IServiceCollection services) =>
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });

    private static IServiceCollection AddHealthCheck(this IServiceCollection services) =>
        services.AddHealthChecks().Services;

    public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>()
            .InitializeDatabasesAsync(cancellationToken);
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IConfiguration config) =>
        builder
            .UseRequestLocalization()
            .UseStaticFiles()
            .UseSecurityHeaders(config)
            .UseFileStorage()
            .UseExceptionMiddleware()
            .UseRouting()
            .UseCorsPolicy()
            .UseAuthentication()
            .UseCurrentUser()

            // .UseMultiTenancy()

            .UseAuthorization()
            .UseRequestLogging(config)
            .UseHangfireDashboard(config)
            .UseOpenApiDocumentation(config);

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapControllers().RequireAuthorization();
        builder.MapHealthCheck();
        builder.MapNotifications();
        return builder;
    }

    private static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapHealthChecks("/api/health");
}