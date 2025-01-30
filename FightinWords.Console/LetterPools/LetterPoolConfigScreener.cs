using FightinWords.Submissions;
using FightinWords.WordLookup;
using OneOf;

namespace FightinWords.Console.LetterPools;

public sealed class LetterPoolConfigScreener : ISubmissionScreener<Word, ILetterPool, Failure>
{
    public const int MinimumPoolSize = 5;

    private static readonly string Recourse = $"""
                                               You must enter either:
                                                 ● The number (≥ {MinimumPoolSize}) of random letters you'd like to play with.
                                                 ● The specific ({MinimumPoolSize} or more) letters you'd like to play with.
                                               """;

    private static Failure GetFailure(string specificError)
    {
        return $"""
                ❌ {specificError}
                {Recourse}
                """;
    }

    public OneOf<ILetterPool, Failure> ScreenInput(Word rawSubmission)
    {
        if (int.TryParse(rawSubmission.ToString(), out var poolSize))
        {
            if (poolSize < MinimumPoolSize)
            {
                return GetFailure($"{poolSize} is less than the minimum pool size of {MinimumPoolSize}.");
            }

            return new ScrabbleLetterPool(Language.English, poolSize);
        }

        if (rawSubmission.Length < MinimumPoolSize)
        {
            return GetFailure(
                $"The pool `{rawSubmission}` contains fewer than the minimum of {MinimumPoolSize} letters.");
        }

        return new FixedLetterPool(rawSubmission);
    }
}