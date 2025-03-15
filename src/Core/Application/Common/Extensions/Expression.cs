using System.Linq.Expressions;
using System.Text.Json;

namespace CleanTib.Application.Common.Extensions;
public static class ExpressionExtensions
{
    public static Expression CreateFilterExpression(
        string logic,
        IEnumerable<Filter> filters,
        ParameterExpression parameter)
    {
        Expression filterExpression = default!;

        foreach (var filter in filters)
        {
            Expression bExpresionFilter;

            if (!string.IsNullOrEmpty(filter.Logic))
            {
                if (filter.Filters is null) throw new CustomException("The Filters attribute is required when declaring a logic");
                bExpresionFilter = CreateFilterExpression(filter.Logic, filter.Filters, parameter);
            }
            else
            {
                var filterValid = BaseFilterExtensions.GetValidFilter(filter);
                bExpresionFilter = CreateFilterExpression(filterValid.Field!, filterValid.Operator!, filterValid.Value, parameter);
            }

            filterExpression = filterExpression is null ? bExpresionFilter : CombineFilter(logic, filterExpression, bExpresionFilter);
        }

        return filterExpression;
    }

    public static Expression CreateFilterExpression(
        string field,
        string filterOperator,
        object? value,
        ParameterExpression parameter)
    {
        var propertyExpresion = GetPropertyExpression(field, parameter);
        var valueExpresion = GeValuetExpression(field, value, propertyExpresion.Type);
        return CreateFilterExpression(propertyExpresion, valueExpresion, filterOperator);
    }

    public static Expression CreateFilterExpression(
        Expression memberExpression,
        Expression constantExpression,
        string filterOperator)
    {
        if (memberExpression.Type == typeof(string))
        {
            constantExpression = Expression.Call(constantExpression, "ToLower", null);
            memberExpression = Expression.Call(memberExpression, "ToLower", null);
        }

        return filterOperator switch
        {
            FilterOperator.EQ => Expression.Equal(memberExpression, constantExpression),
            FilterOperator.NEQ => Expression.NotEqual(memberExpression, constantExpression),
            FilterOperator.LT => Expression.LessThan(memberExpression, constantExpression),
            FilterOperator.LTE => Expression.LessThanOrEqual(memberExpression, constantExpression),
            FilterOperator.GT => Expression.GreaterThan(memberExpression, constantExpression),
            FilterOperator.GTE => Expression.GreaterThanOrEqual(memberExpression, constantExpression),
            FilterOperator.CONTAINS => Expression.Call(memberExpression, "Contains", null, constantExpression),
            FilterOperator.STARTSWITH => Expression.Call(memberExpression, "StartsWith", null, constantExpression),
            FilterOperator.ENDSWITH => Expression.Call(memberExpression, "EndsWith", null, constantExpression),
            _ => throw new CustomException("Filter Operator is not valid."),
        };
    }

    public static Expression CombineFilter(
        string filterOperator,
        Expression bExpresionBase,
        Expression bExpresion) => filterOperator switch
        {
            FilterLogic.AND => Expression.And(bExpresionBase, bExpresion),
            FilterLogic.OR => Expression.Or(bExpresionBase, bExpresion),
            FilterLogic.XOR => Expression.ExclusiveOr(bExpresionBase, bExpresion),
            _ => throw new ArgumentException("FilterLogic is not valid."),
        };

    public static MemberExpression GetPropertyExpression(
        string propertyName,
        ParameterExpression parameter)
    {
        Expression propertyExpression = parameter;
        foreach (string member in propertyName.Split('.'))
        {
            propertyExpression = Expression.PropertyOrField(propertyExpression, member);
        }

        return (MemberExpression)propertyExpression;
    }

    public static string GetStringFromJsonElement(object value)
        => ((JsonElement)value).GetString()!;

    public static ConstantExpression GeValuetExpression(
        string field,
        object? value,
        Type propertyType)
    {
        if (value == null) return Expression.Constant(null, propertyType);

        if (propertyType.IsEnum)
        {
            string? stringEnum = GetStringFromJsonElement(value);

            if (!Enum.TryParse(propertyType, stringEnum, true, out object? valueparsed)) throw new CustomException(string.Format("Value {0} is not valid for {1}", value, field));

            return Expression.Constant(valueparsed, propertyType);
        }

        if (propertyType == typeof(Guid))
        {
            string? stringGuid = GetStringFromJsonElement(value);

            if (!Guid.TryParse(stringGuid, out Guid valueparsed)) throw new CustomException(string.Format("Value {0} is not valid for {1}", value, field));

            return Expression.Constant(valueparsed, propertyType);
        }

        if (propertyType == typeof(string))
        {
            string? text = GetStringFromJsonElement(value);

            return Expression.Constant(text, propertyType);
        }

        if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
        {
            string? text = GetStringFromJsonElement(value);
            return Expression.Constant(TypeExtensions.ChangeType(text, propertyType), propertyType);
        }

        return Expression.Constant(TypeExtensions.ChangeType(((JsonElement)value).GetRawText(), propertyType), propertyType);
    }
}
