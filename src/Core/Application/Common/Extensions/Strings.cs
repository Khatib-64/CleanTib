using System.Text.RegularExpressions;

namespace CleanTib.Application.Common.Extensions;

public static class StringsExtensions
{
    public static string RemoveInBetweenDuplicatedWhitespaces(this string stringValue)
        => Regex.Replace(stringValue, " {2,}", " ");

    public static bool HasValue(this string stringValue)
        => string.IsNullOrWhiteSpace(stringValue) && string.IsNullOrEmpty(stringValue);
}
