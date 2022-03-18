namespace Heavens.Core.Extension.Extensions;

public static partial class StringExtension
{
    public static string ToLambdaString(this Type exp)
    {
        return exp?.ToString()?.Replace("OrElse", "||").Replace("AndAlso", "&&").Replace("Not", "!");
    }
}
