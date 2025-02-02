using OneOf;

namespace FightinWords.Console;

public static class OneOfExtensions
{
    // public static OfThree<
    //     A2,
    //     B2,
    //     C
    // > FlatMap<A, A2, B, B2, C>(
    //     this OneOf<A, B>      ab,
    //     Func<A, OneOf<A2, C>> ifA,
    //     Func<B, OneOf<B2, C>> ifB
    // )
    // {
    //     if (ab.TryPickT0(out var a, out var b))
    //     {
    //         return ifA(a);
    //     }
    //     else
    //     {
    //         return ifB(b);
    //     }
    // }

    public static OneOf<A2, B2> Map<A, B, A2, B2>(
        this OneOf<A, B> oneOf,
        Func<A, A2>      t0Mapper,
        Func<B, B2>      t1Mapper
    )
    {
        if (oneOf.TryPickT0(out var t0, out var t1))
        {
            return t0Mapper(t0);
        }

        return t1Mapper(t1);
    }

    // public static OneOf<T0, T1> PeekT0<T0, T1>(this OneOf<T0, T1> oneOf, Action<T0> action)
    // {
    //     if (oneOf.TryPickT0(out var t0, out _))
    //     {
    //         action(t0);
    //     }
    //
    //     return oneOf;
    // }

    // public static OneOf<A, B, C> Expand<A, B, C>(this OneOf<A, B> ab, C c) => ab.TryPickT0(out var a, out var b) ? a : b;
    // public static OneOf<A, B, C> Expand<A, B, C>(this OneOf<A, C> ac) => ac.TryPickT0(out var a, out var c) ? a : c;
    // public static OneOf<A, B, C> Expand3<A, B, C>(this OneOf<B,C> bc, A _ = default) => bc.TryPickT0(out var b, out var c) ? b : c;
}