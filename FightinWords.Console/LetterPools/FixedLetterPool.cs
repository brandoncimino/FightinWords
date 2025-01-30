namespace FightinWords.Console.LetterPools;

/// <summary>
/// An <see cref="ILetterPool"/> containing a specific set of <see cref="Letters"/>.
/// </summary>
/// <param name="Letters">The <see cref="Letters"/> that this pool will <b><i>always</i></b> use.</param>
public sealed record FixedLetterPool(Word Letters) : ILetterPool
{
    public int  PoolSize         => Letters.Length;
    public Word LockIn(Random _) => Letters;
}