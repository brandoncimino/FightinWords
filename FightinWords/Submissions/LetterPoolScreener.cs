using OneOf;

namespace FightinWords.Submissions;

public sealed class LetterPoolSubmissionScreener : ISubmissionScreener<string, Word>
{
    public required Word LetterPool        { get; init; }
    public required int  MinimumWordLength { get; init; }

    public OneOf<Word, Failure> ScreenInput(string rawInput)
    {
        string userInput = rawInput;
        userInput = userInput.ToLower();

        if (Word.TryParse(userInput, out var result,
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries) == false)
        {
            return new Failure("You entered some weird junk that we couldn't parse.");
        }

        if (result.Length < MinimumWordLength)
        {
            return new Failure($"You must enter at least {MinimumWordLength} letters.");
        }

        var remainingPool = LetterPool.ToList();
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var letter in result)
        {
            if (remainingPool.Remove(letter) == false)
            {
                return new Failure(LetterPool.Contains(letter)
                    ? $"You've exceeded your `{letter}` budget."
                    : $"The letter `{letter}` isn't in your pool.");
            }
        }

        return result;
    }
}