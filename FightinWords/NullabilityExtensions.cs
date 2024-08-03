using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace FightinWords;

internal static class NullabilityExtensions
{
    public static ImmutableDictionary<K, V> OrEmpty<K, V>(this ImmutableDictionary<K, V>? dictionary)
    {
        return dictionary ?? ImmutableDictionary<K, V>.Empty;
    }

    public static ImmutableList<T> OrEmpty<T>(this ImmutableList<T>? list)
    {
        return list ?? ImmutableList<T>.Empty;
    }

    public static T ThrowIfNull<T>(this T? value, [CallerArgumentExpression(nameof(value))] string _value = "")
    {
        ArgumentNullException.ThrowIfNull(value, _value);
        return value;
    }
}