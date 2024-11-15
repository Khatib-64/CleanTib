using CleanTib.Application.Common.Interfaces;
using CleanTib.Infrastructure.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CleanTib.Infrastructure.Validations;

public static class Extensions
{
    public static IServiceCollection AddBehaviours(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(WhitespaceTrimBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
