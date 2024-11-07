using CleanTib.Application.Common.Extensions;
using CleanTib.Application.Common.Interfaces;
using MediatR;
using System.Reflection;

namespace CleanTib.Application.Common.Behaviors;

/// <summary>
/// Specifies that the associated string property should not undergo trimming operations, 
/// preserving leading, trailing, and in-between whitespace as is.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DoNotTrimAttribute : Attribute;

/// <summary>
/// A pipeline behavior that trims leading and trailing whitespace from string properties 
/// of the command and removes any duplicated whitespaces between words. 
/// Skips properties that have the <see cref="DoNotTrimAttribute"/> attribute.
/// </summary>
/// <typeparam name="TCommand">The type of the command being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response produced by the command.</typeparam>
public class WhitespaceTrimBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
    where TCommand : class, ICommand<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TCommand request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var propertiesOfTypeString = typeof(TCommand)
            .GetProperties()
            .Where(x => x.PropertyType == typeof(string) && x.GetCustomAttribute<DoNotTrimAttribute>() is null);

        foreach (var property in propertiesOfTypeString)
        {
            if (property.GetValue(request) is null)
                continue;

            string newValue = (property.GetValue(request) as string)
                .Trim()
                .RemoveInBetweenDuplicatedWhitespaces();

            property.SetValue(request, newValue);
        }

        return await next();
    }
}
