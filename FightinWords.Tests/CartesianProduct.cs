namespace FightinWords;

public static class CartesianProduct
{
    public static IEnumerable<(A a, B b)> Of<A, B>(ICollection<A> a, ICollection<B> b)
    {
        return a.SelectMany(aa => b.Select(bb => (aa, bb)));
    }

    public static IEnumerable<(A a, B b, C c)> Of<A, B, C>(ICollection<A> a, ICollection<B> b, ICollection<C> c)
    {
        return a.SelectMany(aa =>
            b.SelectMany(bb =>
                c.Select(cc => (aa, bb, cc))
            )
        );
    }
}