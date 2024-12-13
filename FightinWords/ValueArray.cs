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

    public bool Equals(ValueArray<T> other)
    {
        return Values.SequenceEqual(other.Values);
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
        return Values.AsEnumerable().GetEnumerator();
    }
}

public static class ValueArrayExtensions
{
    public static ValueArray<T> ToValueArray<T>(this IEnumerable<T> stuff)
    {
        return new ValueArray<T>([..stuff]);
    }
}