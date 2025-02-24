using System.Diagnostics;
using FightinWords.Submissions;
using FightinWords.WordLookup;
using OneOf;

namespace FightinWords.Console.LetterPools;

public sealed class LetterPoolConfigScreener : ISubmissionScreener<Word, ILetterPool>
{
    public const int MinimumPoolSize = 5;

    public static readonly string Instructions = $"""
                                                  Please enter either:
                                                    ● The number (≥ {MinimumPoolSize}) of random letters you'd like to play with.
                                                    ● The specific ({MinimumPoolSize} or more) letters you'd like to play with.
                                                  """;

    public UserFeedback GetPrompt()
    {
        return new UserFeedback(UserFeedback.FeedbackTone.Neutral, "✍️", Instructions);
    }

    private static Failure GetFailure(string specificError)
    {
        return $"""
                ❌ {specificError}
                {Instructions}
                """;
    }

    public OneOf<ILetterPool, Failure> ScreenInput(Word rawSubmission)
    {
        Debug.WriteLine($"Screening for Letter Pool configuration; {nameof(rawSubmission)}: {rawSubmission}");
        if (int.TryParse(rawSubmission.ToString(), out var poolSize))
        {
            Debug.WriteLine($"User input was an integer: {poolSize}");
            if (poolSize < MinimumPoolSize)
            {
                return GetFailure($"{poolSize} is less than the minimum pool size of {MinimumPoolSize}.");
            }

            return new ScrabbleLetterPool()
            {
                Language = Language.English,
                PoolSize = poolSize
            };
        }

        if (rawSubmission.Length < MinimumPoolSize)
        {
            return GetFailure(
                $"The pool `{rawSubmission}` contains fewer than the minimum of {MinimumPoolSize} letters.");
        }

        return new FixedLetterPool(rawSubmission);
    }
}