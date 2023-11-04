using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CleanTib.Infrastructure.Validations;
public static class Extensions
{
    public static IServiceCollection AddBehaviours(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
