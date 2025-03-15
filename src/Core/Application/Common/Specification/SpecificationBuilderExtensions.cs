using CleanTib.Application.Common.Extensions;
using CleanTib.Domain.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace CleanTib.Application.Common.Specification;

// See https://github.com/ardalis/Specification/issues/53
public static class SpecificationBuilderExtensions
{
    public static ISpecificationBuilder<T> SearchBy<T>(this ISpecificationBuilder<T> query, BaseFilter filter) =>
        query
            .SearchByKeyword(filter.Keyword)
            .AdvancedSearch(filter.AdvancedSearch)
            .AdvancedFilter(filter.AdvancedFilter);

    public static ISpecificationBuilder<T> PaginateBy<T>(this ISpecificationBuilder<T> query, PaginationFilter filter)
    {
        if (filter.PageNumber <= 0)
            filter.PageNumber = 1;

        if (filter.PageSize <= 0)
        {
            if (!filter.GetAllRecords)
                filter.PageSize = 10;
            else
                filter.PageSize = int.MaxValue;
        }

        if (filter.PageNumber > 1)
            query = query.Skip((filter.PageNumber - 1) * filter.PageSize);

        return query
            .Take(filter.PageSize)
            .OrderBy(filter.OrderBy);
    }

    public static IOrderedSpecificationBuilder<T> SearchByKeyword<T>(this ISpecificationBuilder<T> specificationBuilder, string? keyword) =>
        specificationBuilder.AdvancedSearch(new Search { Keyword = keyword });

    public static IOrderedSpecificationBuilder<T> AdvancedSearch<T>(this ISpecificationBuilder<T> specificationBuilder, Search? search)
    {
        if (string.IsNullOrEmpty(search?.Keyword))
            return new OrderedSpecificationBuilder<T>(specificationBuilder.Specification);

        if (search.Fields?.Any() is true)
            return SearchOverSelectedProperties(specificationBuilder, search);

        var deepSearchAttribute = typeof(T).GetCustomAttribute<ClassSupportDeepSearchAttribute>();

        if (deepSearchAttribute == null)
            SearchOnlyInBasePrimitiveProperties(specificationBuilder, search);

        if (deepSearchAttribute != null)
            SearchInDeepNestedProperties(specificationBuilder, search, deepSearchAttribute.Depth);

        return new OrderedSpecificationBuilder<T>(specificationBuilder.Specification);
    }

    private static OrderedSpecificationBuilder<T> SearchOverSelectedProperties<T>(ISpecificationBuilder<T> specificationBuilder, Search? search)
    {
        // Search selected fields (can contain deeper nested fields)
        foreach (string field in search.Fields)
        {
            var paramExpr = Expression.Parameter(typeof(T));
            MemberExpression propertyExpr = ExpressionExtensions.GetPropertyExpression(field, paramExpr);

            specificationBuilder.AddSearchPropertyByKeyword(propertyExpr, paramExpr, search.Keyword);
        }

        return new OrderedSpecificationBuilder<T>(specificationBuilder.Specification);
    }

    private static void SearchOnlyInBasePrimitiveProperties<T>(ISpecificationBuilder<T> specificationBuilder, Search? search)
    {
        var props = typeof(T).GetProperties()
            .Where(prop => (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType) is { } propertyType
                && !propertyType.IsEnum
                && Type.GetTypeCode(propertyType) != TypeCode.Object)
            .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null);

        foreach (var property in props)
        {
            var paramExpr = Expression.Parameter(typeof(T));
            var propertyExpr = Expression.Property(paramExpr, property);
            specificationBuilder.AddSearchPropertyByKeyword(propertyExpr, paramExpr, search?.Keyword!);
        }
    }

    private static void SearchInDeepNestedProperties<T>(ISpecificationBuilder<T> specificationBuilder, Search? search, uint depth)
    {
        var props = typeof(T).GetProperties()
            .Where(prop => (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType) is { } propertyType
                && !propertyType.IsEnum
                && !prop.GetCustomAttributes<NotMappedAttribute>(false).Any()
                && prop.GetCustomAttribute<ColumnSupportDeepSearchAttribute>(false) != null);

        foreach (var property in props)
        {
            var paramExpr = Expression.Parameter(typeof(T));
            var propertyExpr = Expression.Property(paramExpr, property);

            if (property.PropertyType.IsOfType<string>())
                specificationBuilder.AddSearchPropertyByKeyword(propertyExpr, paramExpr, search?.Keyword!);
            else if (depth != 0)
                ChildPropertyDepthFilter(property, propertyExpr, paramExpr, specificationBuilder, search?.Keyword!, depth);
        }
    }

    private static void AddSearchPropertyByKeyword<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        Expression propertyExpr,
        ParameterExpression paramExpr,
        string keyword,
        string operatorSearch = FilterOperator.CONTAINS)
    {
        if (propertyExpr is not MemberExpression memberExpr || memberExpr.Member is not PropertyInfo property)
            throw new ArgumentException("propertyExpr must be a property expression.", nameof(propertyExpr));

        string searchTerm = operatorSearch switch
        {
            FilterOperator.STARTSWITH => $"{keyword.ToLower()}%",
            FilterOperator.ENDSWITH => $"%{keyword.ToLower()}",
            FilterOperator.CONTAINS => $"%{keyword.ToLower()}%",
            _ => throw new ArgumentException("operatorSearch is not valid.", nameof(operatorSearch))
        };

        // Generate lambda [ x => x.Property ] for string properties
        // or [ x => ((object)x.Property) == null ? null : x.Property.ToString() ] for other properties
        Expression selectorExpr =
            property.PropertyType == typeof(string)
                ? propertyExpr
                : Expression.Condition(
                    Expression.Equal(Expression.Convert(propertyExpr, typeof(object)), Expression.Constant(null, typeof(object))),
                    Expression.Constant(null, typeof(string)),
                    Expression.Call(propertyExpr, "ToString", null, null));

        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        Expression callToLowerMethod = Expression.Call(selectorExpr, toLowerMethod!);

        var selector = Expression.Lambda<Func<T, string>>(callToLowerMethod, paramExpr);

        ((List<SearchExpressionInfo<T>>)specificationBuilder.Specification.SearchCriterias)
            .Add(new SearchExpressionInfo<T>(selector, searchTerm, 1));
    }

    public static IOrderedSpecificationBuilder<T> AdvancedFilter<T>(this ISpecificationBuilder<T> specificationBuilder, Filter? filter)
    {
        if (filter is null)
            return new OrderedSpecificationBuilder<T>(specificationBuilder.Specification);

        var parameter = Expression.Parameter(typeof(T));

        Expression binaryExpressionFilter;

        if (!string.IsNullOrEmpty(filter.Logic))
        {
            if (filter.Filters is null) throw new CustomException("The Filters attribute is required when declaring a logic");
            binaryExpressionFilter = ExpressionExtensions.CreateFilterExpression(filter.Logic, filter.Filters, parameter);
        }
        else
        {
            var filterValid = BaseFilterExtensions.GetValidFilter(filter);
            binaryExpressionFilter = ExpressionExtensions.CreateFilterExpression(filterValid.Field!, filterValid.Operator!, filterValid.Value, parameter);
        }

        ((List<WhereExpressionInfo<T>>)specificationBuilder.Specification.WhereExpressions)
            .Add(new WhereExpressionInfo<T>(Expression.Lambda<Func<T, bool>>(binaryExpressionFilter, parameter)));

        return new OrderedSpecificationBuilder<T>(specificationBuilder.Specification);
    }

    public static void ChildPropertyDepthFilter<T>(
        PropertyInfo childProperty,
        MemberExpression parentExpr,
        ParameterExpression parentParam,
        ISpecificationBuilder<T> specificationBuilder,
        string keyword,
        uint depth)
    {
        if (depth == 0) return;

        var childType = childProperty.PropertyType;
        var deepSearchAttribute = childType.GetCustomAttribute<ClassSupportDeepSearchAttribute>();

        if (deepSearchAttribute == null) return;

        var nestedProps = childType.GetProperties()
            .Where(prop => (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType) is { } propertyType
            && !propertyType.IsEnum
            && !prop.GetCustomAttributes<NotMappedAttribute>(false).Any()
            && prop.GetCustomAttribute<ColumnSupportDeepSearchAttribute>(false) != null);

        foreach (var nestedProp in nestedProps)
        {
            var nestedPropertyExpr = Expression.Property(parentExpr, nestedProp);

            if (nestedProp.PropertyType.IsOfType<string>())
                specificationBuilder.AddSearchPropertyByKeyword(nestedPropertyExpr, parentParam, keyword);
            else
                ChildPropertyDepthFilter(nestedProp, nestedPropertyExpr, parentParam, specificationBuilder, keyword, depth - 1);
        }
    }

    public static IOrderedSpecificationBuilder<T> OrderBy<T>(this ISpecificationBuilder<T> specificationBuilder, string[]? orderByFields)
    {
        if (orderByFields is null)
            return new OrderedSpecificationBuilder<T>(specificationBuilder.Specification);

        foreach (var field in ParseOrderBy(orderByFields))
        {
            var paramExpr = Expression.Parameter(typeof(T));

            Expression propertyExpr = paramExpr;
            foreach (string member in field.Key.Split('.'))
                propertyExpr = Expression.PropertyOrField(propertyExpr, member);

            var keySelector = Expression.Lambda<Func<T, object?>>(
                Expression.Convert(propertyExpr, typeof(object)),
                paramExpr);

            ((List<OrderExpressionInfo<T>>)specificationBuilder.Specification.OrderExpressions)
                .Add(new OrderExpressionInfo<T>(keySelector, field.Value));
        }

        return new OrderedSpecificationBuilder<T>(specificationBuilder.Specification);
    }

    private static Dictionary<string, OrderTypeEnum> ParseOrderBy(string[] orderByFields) =>
        new(orderByFields.Select((orderByfield, index) =>
        {
            string[] fieldParts = orderByfield.Split(' ');
            string field = fieldParts[0];
            bool descending = fieldParts.Length > 1 && fieldParts[1].StartsWith("Desc", StringComparison.OrdinalIgnoreCase);
            var orderBy = index == 0
                ? descending ? OrderTypeEnum.OrderByDescending
                                : OrderTypeEnum.OrderBy
                : descending ? OrderTypeEnum.ThenByDescending
                                : OrderTypeEnum.ThenBy;

            return new KeyValuePair<string, OrderTypeEnum>(field, orderBy);
        }));
}