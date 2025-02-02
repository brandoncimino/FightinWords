using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using FightinWords.WordLookup;
using FluentAssertions;
using OneOf;

namespace FightinWords;

public static class TestHelpers
{
    public static Rune           AsRune(this      char c)                  => new(c);
    public static IEnumerable<T> Repeated<T>(this T    element, int count) => Enumerable.Repeat(element, count);

    public static string JoinString<T>(
        this IEnumerable<T> elements,
        string              joiner = "",
        string              prefix = "",
        string              suffix = ""
    ) => prefix + string.Join(joiner, elements) + suffix;

    public static IEnumerable<string> AsEnumerable(this TextElementEnumerator textElementEnumerator)
    {
        while (textElementEnumerator.MoveNext())
        {
            yield return textElementEnumerator.GetTextElement();
        }
    }

    public static int LengthInTextElements(this string str) => new StringInfo(str).LengthInTextElements;

    public static T Print<T>(
        this T value,
        [CallerArgumentExpression(nameof(value))]
        string _value =
            ""
    )
    {
        var valueString = GetValueString(value);
        System.Console.WriteLine($"{_value,10} {valueString}");
        return value;
    }

    private static string GetValueString<T>(T value)
    {
        if (value is null)
        {
            return "◌";
        }

        if (value is string str)
        {
            return str;
        }

        if (value is IEnumerable enumerable)
        {
            var sb = new StringBuilder();
            sb.Append('[');

            bool first = true;
            foreach (var it in enumerable)
            {
                sb.Append(GetValueString(it));
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }
            }

            sb.Append(']');
            return sb.ToString();
        }

        return "" + value;
    }

    public static IImmutableDictionary<string, bool> GetIsProperties<T>(this T c)
    {
        var methods = typeof(T).GetMethods(BindingFlags.Static | BindingFlags.Public)
                               .Where(it => it.GetParameters() is [var t] && t.ParameterType == typeof(T))
                               .Where(it => it.ReturnType == typeof(bool));

        return methods.ToImmutableDictionary(
            it => it.Name,
            it => it.Invoke(null, [c]) as bool? ?? throw new InvalidOperationException()
        );
    }

    public static IEnumerable<(A a, B b)> CartesianProduct<A, B>(ICollection<A> aList, ICollection<B> bList)
    {
        foreach (var a in aList)
        {
            foreach (var b in bList)
            {
                yield return (a, b);
            }
        }
    }

    public static IEnumerable<(A a, B b, C c)> CartesianProduct<A, B, C>(
        ICollection<A> aList,
        ICollection<B> bList,
        ICollection<C> cList
    )
    {
        foreach (var (a, b) in CartesianProduct(aList, bList))
        {
            foreach (var c in cList)
            {
                yield return (a, b, c);
            }
        }
    }

    public static IEnumerable<FieldInfo> GetStaticFields(this Type owner) =>
        owner.GetFields(BindingFlags.Static | BindingFlags.Public);

    public static IEnumerable<object?> ReadStaticFields(this Type owner) =>
        owner.GetStaticFields().Select(it => it.GetValue(null));

    public static ImmutableDictionary<string, TData> GetStaticData<TData>(this Type owner)
    {
        return owner.GetStaticFields()
                    .Where(it => it.FieldType.IsAssignableFrom(typeof(TData)))
                    .ToImmutableDictionary(
                        it => it.Name,
                        it => (TData?)it.GetValue(null) ??
                              throw new InvalidOperationException($"The value of the static field {it} was null!")
                    );
    }

    public static IEnumerable<T> GetStaticData_Flatten<T>(this Type owner)
    {
        return owner.GetStaticFields()
                    .Where(it =>
                        it.FieldType.IsAssignableTo(typeof(T)) || it.FieldType.IsAssignableTo(typeof(IEnumerable<T>)))
                    .Select(it => it.GetValue(null))
                    .SelectMany(it => it switch
                    {
                        T t               => Enumerable.Repeat(t, 1),
                        IEnumerable<T> ts => ts,
                        null              => [],
                        _                 => throw new UnreachableException($"`it` was type: {it.GetType()}")
                    });
    }

    public static IEnumerable<T> GetStaticData<T, ATTR>(this Type owner) where ATTR : Attribute
    {
        return owner.GetStaticFields()
                    .Where(it => it.IsDefined(typeof(ATTR)))
                    .Select(it => it.GetValue(null))
                    .OfType<T>();
    }

    [StackTraceHidden]
    public static void AssertEquals<T>(
        T      actual,
        T      expected,
        string label = "",
        [CallerArgumentExpression(nameof(actual))]
        string _actual = ""
    )
    {
        Assert.That(actual, Is.EqualTo(expected), label.Length == 0 ? _actual : $"[{label}] {_actual}");
    }

    public static MockHttpHandler CreateMockHttpHandler(HttpStatusCode statusCode, string json)
    {
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content    = new StringContent(json, Encoding.UTF8, "application/json")
        };

        return new MockHttpHandler([responseMessage]);
    }

    private sealed class Counter
    {
        private int _count;
        public  int Count => _count;

        public int Increment()
        {
            return Interlocked.Increment(ref _count);
        }
    }

    public static Func<I, O> LimitInvocations<I, O>(
        Func<I, O> func,
        int        limit = 1,
        [CallerArgumentExpression(
            nameof(func))]
        string _func = ""
    )
    {
        var counter = new Counter();
        return input =>
        {
            var invocations = counter.Increment();
            if (invocations > 1)
            {
                throw new InvalidOperationException(
                    $"`{_func}` has already executed {limit} times! This would be invocation #{counter.Count}!");
            }

            return func(input);
        };
    }

    public sealed class Spy<I, O>
    {
        private readonly Func<I, O>                        _functionWithSurveillance;
        private readonly ConcurrentQueue<InvocationReport> _intel = new();

        public ImmutableList<InvocationReport> Intel => _intel.ToImmutableList();

        public sealed record InvocationReport(I Input, OneOf<O, Exception> Result);

        private Spy(Func<I, O> functionWithoutSpying)
        {
            _functionWithSurveillance = input =>
            {
                var result = CatchResult(functionWithoutSpying, input);
                var report = new InvocationReport(input, result);
                _intel.Enqueue(report);
                return report.Result.GetOrThrow();
            };
        }

        public static Func<I, O> Create(Func<I, O> func, out Spy<I, O> spy)
        {
            spy = new Spy<I, O>(func);
            return spy._functionWithSurveillance;
        }
    }

    public static O GetOrThrow<O, E>(this OneOf<O, E> result) where E : Exception
    {
        return result.Match(
            good => good,
            bad => throw bad
        );
    }

    private static OneOf<O, Exception> CatchResult<I, O>(Func<I, O> function, I input)
    {
        try
        {
            return function(input);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public static Func<I, O> CreateSpy<I, O>(this Func<I, O> underSurveillance, out Spy<I, O> spy)
    {
        return Spy<I, O>.Create(underSurveillance, out spy);
    }

    public static bool EqualIgnoringOrder<T>(IEnumerable<T> a, IEnumerable<T> b)
    {
        a.Should().BeEquivalentTo(b);
        throw new NotImplementedException();
    }

    public static Random CreateRandom([CallerMemberName] string seed = "")
    {
        return new Random(seed.Sum(it => it));
    }

    public static T GetRandom<T>(this IList<T> choices, Random random) => choices[random.Next(choices.Count)];

    public static T PickFrom<T>(this Random random, IList<T> choices) => choices[random.Next(choices.Count)];

    public static OneOf<T, Exception> TryCatch<T>(Func<T> function)
    {
        try
        {
            return function();
        }
        catch (Exception e)
        {
            return e;
        }
    }

    private class AlwaysThrowRandomImpl : Random
    {
        public override int Next()
        {
            throw new NotSupportedException();
        }

        public override int Next(int maxValue)
        {
            throw new NotSupportedException();
        }

        public override int Next(int minValue, int maxValue)
        {
            throw new NotSupportedException();
        }

        public override void NextBytes(byte[] buffer)
        {
            throw new NotSupportedException();
        }

        public override void NextBytes(Span<byte> buffer)
        {
            throw new NotSupportedException();
        }

        public override double NextDouble()
        {
            throw new NotSupportedException();
        }

        public override long NextInt64()
        {
            throw new NotSupportedException();
        }

        public override long NextInt64(long maxValue)
        {
            throw new NotSupportedException();
        }

        public override long NextInt64(long minValue, long maxValue)
        {
            throw new NotSupportedException();
        }

        public override float NextSingle()
        {
            throw new NotSupportedException();
        }

        protected override double Sample()
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// An implementation of <see cref="Random"/> that <b>always</b> throws an exception.
    /// </summary>
    public static readonly Random AlwaysThrowRandom = new AlwaysThrowRandomImpl();

    public static T0 GetT0<T0, T1>(this OneOf<T0, T1> oneOf)
    {
        if (oneOf.TryPickT0(out var got, out var not))
        {
            return got;
        }

        throw new InvalidOperationException(
            $"Cannot get {typeof(OneOf<T0, T1>).PrettyType()} as T0 because it is T1: {not}");
    }

    public static string PrettyType(this Type type, int depth = 1)
    {
        if (type.IsGenericType is false)
        {
            return type.Name;
        }

        var genericArgs = type.GetGenericArguments();


        var argStrings = genericArgs.Select(it => depth <= 0 ? "" : PrettyType(it, depth - 1));

        var sb = new StringBuilder(type.Name);
        sb.Append('<');
        sb.AppendJoin(',', argStrings);
        sb.Append('>');
        return sb.ToString();
    }

    public static AssertionException Fail([CallerMemberName] string _caller = "")
    {
        return new AssertionException($"{_caller} should not have been run!");
    }
}