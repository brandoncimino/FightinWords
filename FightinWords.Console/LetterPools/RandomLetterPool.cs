using System.Collections.Immutable;

namespace FightinWords.Console.LetterPools;

/// <summary>
/// A <see cref="ILetterPool"/> that randomly picks a <i>(lowercase)</i> letter from <c>a</c> to <c>z</c> with equal weight.
/// </summary>
public sealed class RandomLetterPool(int poolSize) : ILetterPool
{
    public int PoolSize { get; } = poolSize;

    public Word LockIn(Random random)
    {
        var builder = ImmutableArray.CreateBuilder<Grapheme>(PoolSize);

        for (int i = 0; i < PoolSize; i++)
        {
            var randomChar     = (char)random.Next('a', 'z' + 1);
            var randomGrapheme = Grapheme.Parse(randomChar);
            builder.Add(randomGrapheme);
        }

        return builder.MoveToImmutable();
    }
}