using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace FightinWords;

/// <summary>
/// Helper methods that validate arguments and throw <see cref="ArgumentException"/>s.
/// </summary>
internal static class Preconditions
{
    [UsedImplicitly]
    public static T RejectIf<T>(T value, [RequireStaticDelegate] Predicate<T> failCondition,
                                [CallerArgumentExpression(nameof(value))] string _value = "",
                                [CallerArgumentExpression(nameof(failCondition))]
                                string _failCondition = "")
    {
        if (failCondition(value))
        {
            throw new ArgumentException($"{value} must NOT satisfy: {_failCondition}", _value);
        }

        return value;
    }

    public static ReadOnlySpan<T> RejectIf<T>(ReadOnlySpan<T> span, RoSpanFunc<T, bool> failCondition,
                                              [CallerArgumentExpression(nameof(span))]
                                              string _span = "",
                                              [CallerArgumentExpression(nameof(failCondition))]
                                              string _failCondition = "")
    {
        if (failCondition(span))
        {
            throw new ArgumentException($"{span.ToString()} must NOT satisfy: {_failCondition}", _span);
        }

        return span;
    }

    [UsedImplicitly]
    public static T Require<T>(T value, [RequireStaticDelegate] Predicate<T> requirement,
                               [CallerArgumentExpression(nameof(value))]
                               string _value = "",
                               [CallerArgumentExpression(nameof(requirement))]
                               string _requirement = "")
    {
        if (requirement(value))
        {
            return value;
        }

        throw new ArgumentException($"{value} must satisfy: {_requirement}", _value);
    }

    public delegate OUT RoSpanFunc<T, out OUT>(ReadOnlySpan<T> span);

    [UsedImplicitly]
    public static ReadOnlySpan<T> Require<T>(ReadOnlySpan<T> span, RoSpanFunc<T, bool> requirement,
                                             [CallerArgumentExpression(nameof(span))]
                                             string _span = "",
                                             [CallerArgumentExpression(nameof(requirement))]
                                             string _requirement = "")
    {
        if (requirement(span))
        {
            return span;
        }

        throw new ArgumentException($"{span.ToString()} must satisfy: {_requirement}", _span);
    }
}