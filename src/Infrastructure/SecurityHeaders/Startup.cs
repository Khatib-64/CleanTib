using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace CleanTib.Infrastructure.SecurityHeaders;

internal static class Startup
{
    internal static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, IConfiguration config)
    {
        var settings = config.GetSection(nameof(SecurityHeaderSettings)).Get<SecurityHeaderSettings>();

        if (settings?.Enable is true)
        {
            app.Use(async (context, next) =>
            {
                if (!context.Response.HasStarted)
                {
                    if (!string.IsNullOrWhiteSpace(settings.Headers.XFrameOptions))
                        context.Response.Headers[HeaderNames.XFRAMEOPTIONS] = settings.Headers.XFrameOptions;

                    if (!string.IsNullOrWhiteSpace(settings.Headers.XContentTypeOptions))
                        context.Response.Headers[HeaderNames.XCONTENTTYPEOPTIONS] = settings.Headers.XContentTypeOptions;

                    if (!string.IsNullOrWhiteSpace(settings.Headers.ReferrerPolicy))
                        context.Response.Headers[HeaderNames.REFERRERPOLICY] = settings.Headers.ReferrerPolicy;

                    if (!string.IsNullOrWhiteSpace(settings.Headers.PermissionsPolicy))
                        context.Response.Headers[HeaderNames.PERMISSIONSPOLICY] = settings.Headers.PermissionsPolicy;

                    if (!string.IsNullOrWhiteSpace(settings.Headers.SameSite))
                        context.Response.Headers[HeaderNames.SAMESITE] = settings.Headers.SameSite;

                    if (!string.IsNullOrWhiteSpace(settings.Headers.XXSSProtection))
                        context.Response.Headers[HeaderNames.XXSSPROTECTION] = settings.Headers.XXSSProtection;
                }

                await next();
            });
        }

        return app;
    }
}