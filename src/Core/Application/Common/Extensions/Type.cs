namespace CleanTib.Application.Common.Extensions;

public static class TypeExtensions
{
    public static bool IsOfType<T>(this Type type)
    {
        if (type == typeof(T))
            return true;

        return Nullable.GetUnderlyingType(type) == typeof(T);
    }

    public static dynamic? ChangeType(object value, Type conversion)
    {
        var t = conversion;

        if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
        {
            if (value == null)
            {
                return null;
            }

            t = Nullable.GetUnderlyingType(t);
        }

        return Convert.ChangeType(value, t!);
    }
}
