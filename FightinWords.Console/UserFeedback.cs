using FightinWords.Submissions;
using OneOf;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace FightinWords.Console;

public readonly record struct UserFeedback(UserFeedback.FeedbackTone Tone, string Icon, string Message)
{
    public enum FeedbackTone
    {
        Neutral,
        Positive,
        Negative
    }

    public static UserFeedback Error(Failure failure, SpectreFactory.Theme theme)
    {
        return new UserFeedback(FeedbackTone.Negative, theme.Icons.Error, failure.Reason);
    }

    public static UserFeedback FromJudgement(OneOf<Judgement, Failure> userFeedback, SpectreFactory.Theme theme)
    {
        if (userFeedback.TryPickT0(out var judgement, out var failure))
        {
            return judgement switch
            {
                { Legality: SubmissionManager.Legality.FakeWord } => new UserFeedback(
                    FeedbackTone.Negative,
                    theme.Icons.Rejection,
                    $"\"{judgement.Word}\" is not a real word."
                ),
                { Freshness: SubmissionManager.Freshness.Stale } => new UserFeedback(
                    FeedbackTone.Neutral,
                    theme.Icons.StaleWord,
                    $"\"{judgement.Word}\" has already been submitted."
                ),
                {
                    Legality    : SubmissionManager.Legality.RealWord,
                    Score.Points: var points
                } => new UserFeedback(
                    FeedbackTone.Positive,
                    theme.Icons.Success,
                    $"\"{judgement.Word}\" is worth {points} points."
                ),
                _ => throw new ApplicationException($"Something ain't right with this here judgement: {judgement}")
            };
        }

        return Error(failure, theme);
    }

    public IRenderable ToRenderable(SpectreFactory.Theme theme)
    {
        return Markup.FromInterpolated($"{Icon} {Message}", Tone switch
        {
            FeedbackTone.Neutral  => theme.NeutralFeedback,
            FeedbackTone.Positive => theme.PositiveFeedback,
            FeedbackTone.Negative => theme.NegativeFeedback,
            _                     => throw new InvalidOperationException()
        });
    }
}