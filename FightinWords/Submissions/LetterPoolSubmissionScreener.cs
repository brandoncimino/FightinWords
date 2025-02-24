using OneOf;

namespace FightinWords.Submissions;

public sealed class LetterPoolSubmissionScreener : ISubmissionScreener<Word, Word>
{
    public required Word LetterPool        { get; init; }
    public required int  MinimumWordLength { get; init; }

    public OneOf<Word, Failure> ScreenInput(Word rawInput)
    {
        if (rawInput.Length < MinimumWordLength)
        {
            return new Failure($"You must enter at least {MinimumWordLength} letters.");
        }

        var remainingPool = LetterPool.ToList();
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var letter in rawInput)
        {
            if (remainingPool.Remove(letter) == false)
            {
                return new Failure(LetterPool.Contains(letter)
                    ? $"You've exceeded your `{letter}` budget."
                    : $"The letter `{letter}` isn't in your pool.");
            }
        }

        return rawInput;
    }
}