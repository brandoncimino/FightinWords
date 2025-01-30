namespace FightinWords.Console.LetterPools;

public interface ILetterPool
{
    /// <summary>
    /// The number of <see cref="Word.Letters"/> in this pool.
    /// </summary>
    public int PoolSize { get; }

    /// <summary>
    /// Gets the set of <see cref="Word.Letters"/> that a given game is going to use <i>(which might be <see cref="Random"/>ly generated)</i>.
    /// </summary>
    /// <param name="random">The random number generator that <i>(might)</i> be used to pick <see cref="Word.Letters"/>.</param>
    /// <returns>The <see cref="Word.Letters"/> that we're going to be playing with.</returns>
    public Word LockIn(Random random);
}