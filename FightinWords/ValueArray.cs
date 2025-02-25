using System.Collections;
using System.Collections.Immutable;

namespace FightinWords;

/// <summary>
/// A wrapper around an <see cref="ImmutableArray{T}"/> that uses "value-type semantics" for <see cref="Equals(FightinWords.ValueArray{T})"/> and <see cref="GetHashCode"/> - i.e. two different <see cref="ValueArray{T}"/>s are equal if their items are equal.
/// </summary>
/// <param name="Values">The underlying <see cref="ImmutableArray{T}"/></param>
/// <typeparam name="T"><inheritdoc cref="ImmutableArray{T}"/></typeparam>
public readonly record struct ValueArray<T>(ImmutableArray<T> Values) : IEnumerable<T>
{
    public static readonly ValueArray<T> Empty = ImmutableArray<T>.Empty;

    /// <inheritdoc cref="ImmutableArray{T}.Length"/>
    /// <remarks>
    /// I didn't want to copy methods from <see cref="ImmutableArray{T}"/> into <see cref="ValueArray{T}"/> that would be accessible easily via <see cref="Values"/>,
    /// but <see cref="Length"/> and <see cref="ValueArray{T}.this"/> are included to enable <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns#list-patterns">list patterns</a>.
    /// </remarks>
    public int Length => Values.Length;

    /// <inheritdoc cref="ImmutableArray{T}.this"/>
    /// <remarks><inheritdoc cref="Length"/></remarks>
    public T this[int index] => Values[index];

    public bool Equals(ValueArray<T> other)
    {
        return other.Values.IsDefaultOrEmpty ? Values.IsDefaultOrEmpty : Values.SequenceEqual(other.Values);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        foreach (var it in Values)
        {
            hc.Add(it);
        }

        return hc.ToHashCode();
    }

    public static implicit operator ValueArray<T>(ImmutableArray<T> immutableArray) => new(immutableArray);

    public ImmutableArray<T>.Enumerator GetEnumerator()
    {
        return Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Values.AsEnumerable().GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return Values.IsDefaultOrEmpty ? Enumerable.Empty<T>().GetEnumerator() : Values.AsEnumerable().GetEnumerator();
    }

    public override string ToString()
    {
        return Values switch
        {
            [] => "[]",
            _  => $"[{string.Join(", ", Values)}]"
        };
    }
}

public static class ValueArrayExtensions
{
    public static ValueArray<T> ToValueArray<T>(this IEnumerable<T> stuff)
    {
        return new ValueArray<T>([..stuff]);
    }

    public static bool IsNullOrEmpty<T>(this ValueArray<T>? stuff)
    {
        return stuff?.Values.IsDefaultOrEmpty ?? true;
    }
}