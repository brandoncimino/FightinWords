using System.Text;
using FightinWords.WordLookup;

namespace FightinWords.Scoring;

/// <summary>
/// Takes in some text and gives you some points.
/// </summary>
/// <example>
/// The classic example is the <see cref="ScrabbleScorer"/>, which assigns a certain value to each letter and totals it up.
/// </example>
/// <remarks>
/// Specifically does NOT care about whether a word is considered "valid" - that's the responsibility of <see cref="IWordLookup"/>.
/// </remarks>
public interface IScorer
{
    public int ComputeScore(ReadOnlySpan<Rune> word, Language language);

    public int ComputeScore(SpanRuneEnumerator word, Language language);
}

public static class ScorerExtensions
{
    public static int ComputeScore(this IScorer scorer, string word, Language language)
    {
        return scorer.ComputeScore(word.AsSpan().EnumerateRunes(), language);
    }
}