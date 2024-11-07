using System.Text.RegularExpressions;

namespace CleanTib.Application.Common.Extensions;

public static class StringsExtensions
{
    public static string RemoveInBetweenDuplicatedWhitespaces(this string stringValue)
        => Regex.Replace(stringValue, " {2,}", " ");
}
