namespace CleanTib.Application.Common.Extensions;

public static class TypeExtensions
{
    public static bool IsOfType<T>(this Type type)
    {
        if (type == typeof(T))
            return true;

        var underlayingType = Nullable.GetUnderlyingType(type);

        return underlayingType == typeof(T);
    }
}
