using System.Collections.Immutable;
using OneOf;
using OneOf.Types;

namespace FightinWords.Submissions;

public sealed class LetterPoolSubmissionScreener : ISubmissionScreener<string, Word, LetterPoolSubmissionScreener.FailedScreening>
{
    public required Word LetterPool        { get; init; }
    public required int  MinimumWordLength { get; init; }

    public OneOf<Word, FailedScreening> ScreenInput(string rawInput)
    {
        string userInput = rawInput;
        userInput = userInput.ToLower();

        if (Word.TryParse(userInput, out var result,
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries) == false)
        {
            return new FailedScreening("You entered some weird junk that we couldn't parse.");
        }

        if (result.Length < MinimumWordLength)
        {
            return new FailedScreening($"You must enter at least {MinimumWordLength} letters.");
        }

        var remainingPool = LetterPool.ToList();
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var letter in result)
        {
            if (remainingPool.Remove(letter) == false)
            {
                return new FailedScreening(LetterPool.Contains(letter)
                    ? $"You've exceeded your `{letter}` budget."
                    : $"The letter `{letter}` isn't in your pool.");
            }
        }

        return result;
    }

    public record FailedScreening(string Reason);
}