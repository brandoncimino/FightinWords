using System.ComponentModel;
using System.Diagnostics;
using OneOf;

namespace FightinWords.Console;

public enum WhichOfThree
{
    A,
    B,
    C
}

public readonly struct OfThree<A, B, C>
{
    private readonly A?           _a;
    private readonly B?           _b;
    private readonly C?           _c;
    public           WhichOfThree HasWhich { get; }

    private OfThree(A? a, B? b, C? c, WhichOfThree hasWhich)
    {
        (_a, _b, _c) = hasWhich switch
        {
            WhichOfThree.A => (Preconditions.RequireNotNull(a), Preconditions.RequireDefault(b),
                Preconditions.RequireDefault(c)),
            WhichOfThree.B => (Preconditions.RequireDefault(a), Preconditions.RequireNotNull(b),
                Preconditions.RequireDefault(c)),
            WhichOfThree.C => (Preconditions.RequireDefault(a), Preconditions.RequireDefault(b),
                Preconditions.RequireNotNull(c)),
            _ => throw new ArgumentOutOfRangeException(nameof(hasWhich), hasWhich, null)
        };

        HasWhich = hasWhich;
    }

    public static OfThree<A, B, C> FromA(A a) => new(a, default, default, WhichOfThree.A);
    public static OfThree<A, B, C> FromB(B b) => new(default, b, default, WhichOfThree.B);
    public static OfThree<A, B, C> FromC(C c) => new(default, default, c, WhichOfThree.C);

    public static implicit operator OfThree<A, B, C>(A a) => FromA(a);
    public static implicit operator OfThree<A, B, C>(B b) => FromB(b);
    public static implicit operator OfThree<A, B, C>(C c) => FromC(c);

    public static implicit operator OfThree<A, B, C>(OneOf<A, B> ab) => ab.TryPickT0(out var a, out var b) ? a : b;
    public static implicit operator OfThree<A, B, C>(OneOf<A, C> ac) => ac.TryPickT0(out var a, out var c) ? a : c;
    public static implicit operator OfThree<A, B, C>(OneOf<B, C> bc) => bc.TryPickT0(out var b, out var c) ? b : c;

    public T Switch<T>(
        Func<A, T> a,
        Func<B, T> b,
        Func<C, T> c
    )
    {
        return HasWhich switch
        {
            WhichOfThree.A => a(_a!),
            WhichOfThree.B => b(_b!),
            WhichOfThree.C => c(_c!),
            _ => throw new InvalidEnumArgumentException(nameof(HasWhich), (int)HasWhich, typeof(WhichOfThree))
        };
    }

    public OfThree<A2, B2, C2> Map<A2, B2, C2>(Func<A, A2> a, Func<B, B2> b, Func<C, C2> c)
    {
        return Switch<OfThree<A2, B2, C2>>(
            aa => a(aa),
            bb => b(bb),
            cc => c(cc)
        );
    }

    public override string ToString()
    {
        return HasWhich switch
        {
            WhichOfThree.A => $"🅰️ {_a}",
            WhichOfThree.B => $"🅱️ {_b}",
            WhichOfThree.C => $"©️ {_c}",
            _              => throw new UnreachableException()
        };
    }
}