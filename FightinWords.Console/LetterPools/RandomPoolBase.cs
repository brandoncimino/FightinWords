using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using FightinWords.WordLookup;
using JetBrains.Annotations;

namespace FightinWords.Console.LetterPools;

/// <summary>
/// Base class for randomly-generated <see cref="ILetterPool"/>s like <see cref="RandomLetterPool"/> and <see cref="ScrabbleLetterPool"/>.
/// </summary>
/// <typeparam name="TState">Some extra stuff that needs to be passed around to <see cref="GetRandomLetter"/>, etc., and might be modified.
/// For example, a <see cref="ScrabbleLetterPool"/> needs to track how many letter tiles are left to draw.</typeparam>
public abstract class RandomPoolBase<TState> : ILetterPool
{
    public const int    DefaultPoolSize      = 6;
    public const int    DefaultMinimumVowels = 1;
    public const string Vowels               = "aeiouy";

    /// <summary>
    /// The number of <see cref="Grapheme"/>s in the pool.
    /// </summary>
    [ValueRange(1, long.MaxValue)]
    public int PoolSize { get; init; } = DefaultPoolSize;

    /// <summary>
    /// The <b>minimum</b> number of <see cref="Phonology.Vowel"/>s <i>(and <see cref="Phonology.Semivowel"/>s)</i> that the pool is guaranteed to contain.
    /// </summary>
    [NonNegativeValue]
    public int MinimumVowels { get; init; } = DefaultMinimumVowels;

    /// <summary>
    /// The <see cref="Language"/> that this pool is made for, which determines things like:
    /// <ul>
    /// <li>What are the possible <see cref="Grapheme"/>s</li>
    /// <li>What letters are <see cref="Phonology.Vowel"/>s</li>
    /// </ul>
    /// </summary>
    public Language Language { get; init; } = Language.English;

    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
    public virtual Word LockIn(Random random)
    {
        Preconditions.Require(PoolSize      > 0);
        Preconditions.Require(MinimumVowels >= 0);
        Preconditions.Require(MinimumVowels <= PoolSize,
            _condition: $"A pool of {PoolSize} letters could never fit {MinimumVowels} vowels!");

        var state = CreateNewState();

        var builder = ImmutableArray.CreateBuilder<Grapheme>(PoolSize);
        for (int i = 0; i < PoolSize; i++)
        {
            var randomGrapheme = GetRandomLetter(random, state);
            builder.Add(randomGrapheme);
        }

        if (MinimumVowels > 0)
        {
            EnsureMinimumVowels(random, builder, state);
        }

        return builder.MoveToImmutable();
    }

    private void EnsureMinimumVowels(Random random, ImmutableArray<Grapheme>.Builder builder, TState state)
    {
        var vowelCount = builder.Count(it => it.GetPhonology(Language) is Phonology.Vowel or Phonology.Semivowel);
        var extraVowelsNeeded = MinimumVowels - vowelCount;
        int nextConsonantIndex = 0;
        for (int i = 0; i < extraVowelsNeeded; i++)
        {
            nextConsonantIndex = NextConsonantIndex(builder, nextConsonantIndex, Language);
            var nextConsonant = builder[nextConsonantIndex];
            var vowel         = ExchangeForVowel(random, state, nextConsonant);
            builder[nextConsonantIndex] = vowel;
        }
    }

    private static int NextConsonantIndex(IReadOnlyList<Grapheme> graphemes, int startFrom, Language language)
    {
        for (int i = startFrom; i < graphemes.Count; i++)
        {
            if (graphemes[i].GetPhonology(language) is Phonology.Vowel or Phonology.Semivowel)
            {
                return i;
            }
        }

        return -1;
    }

    public abstract TState CreateNewState();

    public abstract Grapheme GetRandomLetter(Random random, TState state);

    public abstract Grapheme ExchangeForVowel(Random random, TState state, Grapheme toBeExchanged);
}