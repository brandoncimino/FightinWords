using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace FightinWords;

/// <summary>
/// Helper methods that validate arguments and throw <see cref="ArgumentException"/>s.
/// </summary>
[StackTraceHidden]
internal static class Preconditions
{
    [UsedImplicitly]
    public static T RejectIf<T>(
        T                                    value,
        [RequireStaticDelegate] Predicate<T> failCondition,
        [CallerArgumentExpression(nameof(value))]
        string _value = "",
        [CallerArgumentExpression(nameof(failCondition))]
        string _failCondition = ""
    )
    {
        if (failCondition(value))
        {
            throw new ArgumentException($"{value} must NOT satisfy: {_failCondition}", _value);
        }

        return value;
    }

    public static ReadOnlySpan<T> RejectIf<T>(
        ReadOnlySpan<T>     span,
        RoSpanFunc<T, bool> failCondition,
        [CallerArgumentExpression(nameof(span))]
        string _span = "",
        [CallerArgumentExpression(nameof(failCondition))]
        string _failCondition = ""
    )
    {
        if (failCondition(span))
        {
            throw new ArgumentException($"{span.ToString()} must NOT satisfy: {_failCondition}", _span);
        }

        return span;
    }

    [UsedImplicitly]
    public static T Require<T>(
        T                                    value,
        [RequireStaticDelegate] Predicate<T> requirement,
        [CallerArgumentExpression(nameof(value))]
        string _value = "",
        [CallerArgumentExpression(nameof(requirement))]
        string _requirement = ""
    )
    {
        if (requirement(value))
        {
            return value;
        }

        throw new ArgumentException($"{value} must satisfy: {_requirement}", _value);
    }

    /// <param name="condition">the thing that we are checking <i>(📎 ideally, this should be the full expression, e.g. <c>index >= 0</c>)</i></param>
    /// <param name="requirement">the thing that we want the <see cref="condition"/> to equal</param>
    /// <param name="_condition">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <returns><see cref="condition"/>, in case you care</returns>
    /// <exception cref="ArgumentException">if <see cref="condition"/> != <see cref="requirement"/></exception>
    [UsedImplicitly]
    public static bool Require(
        bool condition,
        bool requirement = true,
        [CallerArgumentExpression(nameof(condition))]
        string _condition = ""
    )
    {
        if (condition == requirement)
        {
            return condition;
        }

        throw new ArgumentException(
            $"{FormatArgumentExpression(condition, _condition)} must evaluate to {requirement}!", _condition);
    }

    /// <summary>
    /// Asserts that <see cref="value"/> equals <paramref name="requirement"/> <i>(via <see cref="EqualityComparer{T}.Default"/>)</i>.
    /// </summary>
    /// <param name="value">the thing we're checking</param>
    /// <param name="requirement">the thing we want <paramref name="value"/> to equal</param>
    /// <param name="paramName">the <see cref="ArgumentException.ParamName"/> that we are checking <i>(📎 defaults to <see cref="CallerArgumentExpressionAttribute"/> for <paramref name="value"/>)</i></param>
    /// <param name="_requirement">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <typeparam name="T">the type of <paramref name="value"/></typeparam>
    /// <returns><paramref name="value"/>, in case you need it</returns>
    /// <exception cref="ArgumentException">if <paramref name="value"/> does not <see cref="EqualityComparer{T}.Equals(T?,T?)"/> <see cref="requirement"/></exception>
    [UsedImplicitly]
    public static T Require<T>(
        T value,
        T requirement,
        [CallerArgumentExpression(nameof(value))]
        string paramName = "",
        [CallerArgumentExpression(nameof(requirement))]
        string _requirement = ""
    )
    {
        if (EqualityComparer<T>.Default.Equals(value, requirement))
        {
            return value;
        }

        throw new ArgumentException($"{value} must equal {FormatArgumentExpression(requirement, _requirement)}",
            paramName);
    }

    public delegate OUT RoSpanFunc<T, out OUT>(ReadOnlySpan<T> span);

    /// <summary>
    /// Asserts that <paramref name="span"/> satisfies <paramref name="predicate"/>.
    /// </summary>
    /// <param name="span">the thing we're checking</param>
    /// <param name="predicate">the condition that should evaluate to <c>true</c> when we give it the <paramref name="span"/></param>
    /// <param name="_span">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_predicate"><see cref="CallerArgumentExpressionAttribute"/></param>
    /// <typeparam name="T">the type of the elements in the <paramref name="span"/></typeparam>
    /// <returns><paramref name="span"/>, in case you want it</returns>
    /// <exception cref="ArgumentException">if <paramref name="predicate"/> returns <c>false</c></exception>
    [UsedImplicitly]
    public static ReadOnlySpan<T> Require<T>(
        ReadOnlySpan<T>     span,
        RoSpanFunc<T, bool> predicate,
        [CallerArgumentExpression(nameof(span))]
        string _span = "",
        [CallerArgumentExpression(nameof(predicate))]
        string _predicate = ""
    )
    {
        if (predicate(span))
        {
            return span;
        }

        throw new ArgumentException($"{span.FormatSpan()} must satisfy: {_predicate}", _span);
    }

    /// <summary>
    /// Identical to <see cref="ArgumentNullException.ThrowIfNull(object?,string?)"/>, but returns the <i>(now known to be non-null)</i>
    /// <paramref name="argument"/>.
    /// </summary>
    /// <param name="argument">something that <i>might</i> be <c>null</c></param>
    /// <param name="paramName">the <see cref="ArgumentNullException.ParamName"/> <i>(defaults to <see cref="CallerArgumentExpressionAttribute"/>)</i></param>
    /// <typeparam name="T">the type of <paramref name="argument"/></typeparam>
    /// <returns><paramref name="argument"/>, which we now know is definitely not <c>null</c> <i>(and is therefore a non-nullable type)</i></returns>
    public static T RequireNotNull<T>(
        T? argument,
        [CallerArgumentExpression(nameof(argument))]
        string paramName = ""
    )
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        return argument;
    }

    /// <summary>
    /// Asserts that <paramref name="argument"/> equals <c>default(</c><typeparamref name="T"/><c>)</c>
    /// </summary>
    /// <param name="argument">something that <i>might</i> be <c>null</c></param>
    /// <param name="paramName">the <see cref="ArgumentOutOfRangeException.ParamName"/> <i>(defaults to <see cref="CallerArgumentExpressionAttribute"/>)</i></param>
    /// <typeparam name="T">the type of <paramref name="argument"/></typeparam>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="argument"/> does NOT equal <c>default(</c><typeparamref name="T"/><c>)</c></exception>
    public static T? RequireDefault<T>(
        T? argument,
        [CallerArgumentExpression(nameof(argument))]
        string paramName = ""
    )
    {
        if (EqualityComparer<T>.Default.Equals(argument, default))
        {
            return default;
        }

        throw new ArgumentOutOfRangeException(paramName, argument,
            $"Must be the default({typeof(T)}), {default(T)?.ToString() ?? "null"}");
    }

    #region Formatting

    private static string? FormatValue<T>(T value)
    {
        return value switch
        {
            null => "⛔",
            _    => value.ToString()
        };
    }

    private static string FormatSpan<T>(this ReadOnlySpan<T> span)
    {
        if (typeof(T) == typeof(char))
        {
            return span.ToString();
        }

        var sb = new StringBuilder();

        sb.Append('[');

        foreach (var it in span)
        {
            sb.Append(it);
        }

        sb.Append(']');
        return sb.ToString();
    }


    private static string FormatArgumentExpression<T>(
        T      value,
        string expression
    )
    {
        var valueString = FormatValue(value);
        if (expression.Equals(valueString))
        {
            return expression;
        }

        return $"{expression} {valueString}";
    }

    #endregion
}