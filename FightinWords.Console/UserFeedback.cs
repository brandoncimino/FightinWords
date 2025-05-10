using System.Diagnostics;
using FightinWords.Console.Rendering;
using FightinWords.Submissions;
using OneOf;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace FightinWords.Console;

public readonly record struct UserFeedback(UserFeedback.FeedbackTone Tone, UserFeedback.FeedbackIcon? Icon, string Message)
{
    public enum FeedbackTone
    {
        Neutral,
        Positive,
        Negative
    }

    public enum FeedbackIcon
    {
        Error,
        Rejection,
        Success,
        StaleWord,

        OriginalSorting,
        AlphabeticalSorting,
        PhonologicalSorting,
        RandomSorting,

        WritingPrompt,
    }

    public static UserFeedback Error(Failure failure)
    {
        return new UserFeedback(FeedbackTone.Negative, FeedbackIcon.Error, failure.Reason);
    }

    public static UserFeedback FromJudgement(OneOf<Judgement, Failure> userFeedback)
    {
        if (userFeedback.TryPickT0(out var judgement, out var failure))
        {
            return judgement switch
            {
                { Legality: SubmissionManager.Legality.FakeWord } => new UserFeedback(
                    FeedbackTone.Negative,
                    FeedbackIcon.Rejection,
                    $"\"{judgement.Word}\" is not a real word."
                ),
                { Freshness: SubmissionManager.Freshness.Stale } => new UserFeedback(
                    FeedbackTone.Neutral,
                    FeedbackIcon.StaleWord,
                    $"\"{judgement.Word}\" has already been submitted."
                ),
                {
                    Legality    : SubmissionManager.Legality.RealWord,
                    Score.Points: var points
                } => new UserFeedback(
                    FeedbackTone.Positive,
                    FeedbackIcon.Success,
                    $"\"{judgement.Word}\" is worth {points} points."
                ),
                _ => throw new ApplicationException($"Something ain't right with this here judgement: {judgement}")
            };
        }

        return Error(failure);
    }

    public static UserFeedback FromLetterSorting(LetterPoolDisplay.LetterSorting letterSorting)
    {
        var (icon, message) = letterSorting switch
        {
            LetterPoolDisplay.LetterSorting.AsOriginallyGiven => (FeedbackIcon.OriginalSorting, "Letters sorted as originally given."),
            LetterPoolDisplay.LetterSorting.Alphabetical      => (FeedbackIcon.AlphabeticalSorting, "Letters sorted alphabetically."),
            LetterPoolDisplay.LetterSorting.Phonological      => (FeedbackIcon.PhonologicalSorting, "Letters sorted phonologically (vowel > semivowel > consonant)."),
            LetterPoolDisplay.LetterSorting.Random            => (FeedbackIcon.RandomSorting, "Letters shuffled."),
            _                                                 => throw new ArgumentOutOfRangeException(nameof(letterSorting), letterSorting, null)
        };

        return new UserFeedback(FeedbackTone.Neutral, icon, message);
    }

    public IRenderable ToRenderable(SpectreFactory.Theme theme)
    {
        var iconString = Icon switch
        {
            FeedbackIcon.Error               => theme.Icons.Error,
            FeedbackIcon.Rejection           => theme.Icons.Rejection,
            FeedbackIcon.Success             => theme.Icons.Success,
            FeedbackIcon.StaleWord           => theme.Icons.StaleWord,
            FeedbackIcon.OriginalSorting     => theme.Icons.OriginalSorting,
            FeedbackIcon.AlphabeticalSorting => theme.Icons.AlphabeticalSorting,
            FeedbackIcon.PhonologicalSorting => theme.Icons.PhonologicalSorting,
            FeedbackIcon.RandomSorting       => theme.Icons.RandomSorting,
            FeedbackIcon.WritingPrompt       => theme.Icons.WritingPrompt,
            null                             => null,
            _                                => throw new UnreachableException()
        };

        if (iconString is not null)
        {
            iconString += " ";
        }

        return Markup.FromInterpolated($"{iconString} {Message}", Tone switch
        {
            FeedbackTone.Neutral  => theme.NeutralFeedback,
            FeedbackTone.Positive => theme.PositiveFeedback,
            FeedbackTone.Negative => theme.NegativeFeedback,
            _                     => throw new UnreachableException()
        });
    }
}