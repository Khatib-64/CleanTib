using CleanTib.Application.Identity.Tokens;
using NSwag;
using NSwag.Generation.Processors.Contexts;
using NSwag.Generation.Processors;
using System.Reflection;
using NJsonSchema;

namespace CleanTib.Infrastructure.OpenApi;

public class DefaultValuesOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var tokenRequestType = typeof(TokenRequest);

        if (context.OperationDescription.Operation.Parameters != null)
        {
            foreach (var parameter in context.OperationDescription.Operation.Parameters)
            {
                if (parameter.Name == "request" && parameter.Schema.Type == JsonObjectType.Object)
                {
                    var properties = parameter.Schema.Properties;

                    foreach (var property in properties)
                    {
                        var defaultValue = GetDefaultValue(property.Key);
                        if (defaultValue != null)
                        {
                            var propertyName = property.Key;
                            context.OperationDescription.Operation.Parameters
                                .First(p => p.Name == propertyName)
                                .Description = $"Default {propertyName}: {defaultValue}";
                        }
                    }
                }
            }
        }

        return true;
    }

    private object GetDefaultValue(string propertyName)
    {
        if (propertyName == "Email")
        {
            return "admin@root.com";
        }
        else if (propertyName == "Password")
        {
            return "P@ssw0rd";
        }

        return null;
    }
}