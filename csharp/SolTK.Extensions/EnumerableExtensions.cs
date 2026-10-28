namespace SolTK.Extensions;
using System.Diagnostics.CodeAnalysis;

public static class EnumerableExtensions
{
    public static bool TryFirst<T>(this IEnumerable<T> source, [MaybeNullWhen(false)] out T result)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source is IList<T> list)
            if (list.Count > 0)
            {
                result = list[0];
                return true;
            }
        
        
        foreach (var item in source)
        {
            result = item;
            return true;
        }

        result = default;
        return false;
    }
}