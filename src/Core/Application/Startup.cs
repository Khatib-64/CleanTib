using System.Reflection;
using CleanTib.Application.Demo;
using Microsoft.Extensions.DependencyInjection;

namespace CleanTib.Application;

public static class Startup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetAssembly(typeof(Startup));

        return services
            .AddValidatorsFromAssembly(assembly)
            .AddMediatR(typeof(DemooCommandHandler).GetTypeInfo().Assembly);
    }
}