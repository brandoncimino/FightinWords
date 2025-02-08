using System.Collections.Immutable;
using FightinWords.Console.Rendering;
using FightinWords.Scoring;
using FightinWords.Submissions;
using FightinWords.WordLookup.Wiktionary;
using Spectre.Console.Rendering;

namespace FightinWords.Console;

/// <summary>
/// Handles <see cref="Word"/> submissions and issues <see cref="Judgement"/>s. 
/// </summary>
/// <remarks>
/// The <see cref="GameDirector"/> delegates to this while in-game.</remarks>
public sealed class GameReferee
{
    #region Game state

    private readonly SharedResources   _sharedResources;
    private readonly SubmissionManager _submissionManager;
    private readonly LetterPoolDisplay _letterPoolDisplay;

    #endregion

    private GameReferee(
        SharedResources   sharedResources,
        SubmissionManager submissionManager,
        LetterPoolDisplay letterPoolDisplay
    )
    {
        _sharedResources   = sharedResources;
        _submissionManager = submissionManager;
        _letterPoolDisplay = letterPoolDisplay;
    }

    public static GameReferee StartGame(
        SharedResources sharedResources
    )
    {
        return new GameReferee(
            sharedResources,
            new SubmissionManager
            {
                Scorer     = new ScrabbleScorer(),
                WordLookup = new WiktionaryClient(),
                SubmissionScreener = new LetterPoolSubmissionScreener()
                {
                    LetterPool        = sharedResources.GamePlan.ProgenitorPool,
                    MinimumWordLength = sharedResources.GamePlan.MinimumWordLength
                }
            },
            LetterPoolDisplay.Create(sharedResources)
        );
    }

    public UserFeedback JudgeWord(Word word)
    {
        var judgement = _submissionManager.SubmitWord(word, _sharedResources.GamePlan.Language);
        return UserFeedback.FromJudgement(judgement, _sharedResources.GamePlan.Theme);
    }

    public ImmutableDictionary<LangWord, SubmissionManager.WordRating?> GetCurrentSubmissions() =>
        _submissionManager.GetCurrentSubmissions();

    public ImmutableArray<Judgement> GetCurrentJudgementHistory() => _submissionManager.GetCurrentJudgementHistory();

    public UserFeedback GetPrompt()
    {
        return new UserFeedback(
            UserFeedback.FeedbackTone.Positive,
            "✍️",
            $"Enter a word that is at least {_sharedResources.GamePlan.MinimumWordLength} letters long."
        );
    }

    public IRenderable ToRenderable()
    {
        return SpectreFactory.RenderScoreBoard(
            _submissionManager.GetCurrentSubmissions(),
            _submissionManager.GetCurrentJudgementHistory(),
            _letterPoolDisplay,
            _sharedResources
        );
    }
}