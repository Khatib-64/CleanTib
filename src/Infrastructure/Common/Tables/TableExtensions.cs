using CleanTib.Domain.Common.Attributes;
using CleanTib.Infrastructure.Common.Extensions;
using System.Reflection;

namespace CleanTib.Infrastructure.Common;

public static class TableExtensions
{
    public static Table ConvertToTable<T>(this IList<T> data, string? title = default)
    {
        var properties = new List<string>();
        var propertiesInfo = typeof(T).GetProperties();

        bool hasTakeThisPropertyAttribute = propertiesInfo.Any(property => property.GetCustomAttribute<TakeThisPropertyAttribute>() != null);

        if (hasTakeThisPropertyAttribute)
        {
            propertiesInfo = propertiesInfo
                .Where(property => property.GetCustomAttribute<TakeThisPropertyAttribute>() != null)
                .ToArray();
        }

        foreach (var property in propertiesInfo)
        {
            properties.Add(property.Name);
        }

        var rows = new List<List<string>>();

        foreach (object item in data)
        {
            var record = new List<string>();

            foreach (var propertyInfo in propertiesInfo)
            {
                if (propertyInfo.GetValue(item) is null)
                {
                    record.Add("-");
                    continue;
                }

                if (propertyInfo.PropertyType.IsOfType<Enum>())
                {
                    // Do somthing to 'result', return it translated to a secific language maybe?
                    string result = ((Enum)propertyInfo.GetValue(item)!).ToString();
                    record.Add(result);
                    continue;
                }

                if (propertyInfo.PropertyType.IsOfType<bool>())
                {
                    string result = ((bool)propertyInfo.GetValue(item)!).ToString();
                    record.Add(result);
                    continue;
                }

                record.Add(propertyInfo.GetValue(item)!.ToString()!);
            }

            rows.Add(record);
        }

        return new Table
        {
            Title = title,
            Columns = properties,
            Rows = rows
        };
    }
}