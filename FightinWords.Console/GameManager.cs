using System.Text;
using FightinWords.Scoring;
using FightinWords.Submissions;
using FightinWords.WordLookup.Wiktionary;
using OneOf;
using Spectre.Console;

namespace FightinWords.Console;

public sealed class GameManager
{
    #region Game state

    private readonly SharedResources                                                 _sharedResources;
    private readonly LetterPoolDisplay                                               _letterPoolDisplay;
    private readonly SubmissionManager                                               _submissionManager;
    private          OneOf<Judgement, LetterPoolSubmissionScreener.FailedScreening>? _previousSubmission;

    #endregion

    private GameManager(
        SharedResources   sharedResources,
        LetterPoolDisplay letterPoolDisplay,
        SubmissionManager submissionManager
    )
    {
        _sharedResources   = sharedResources;
        _letterPoolDisplay = letterPoolDisplay;
        _submissionManager = submissionManager;
    }

    public static GameManager StartGame(
        GamePlan gamePlan,
        Random   random
    )
    {
        System.Console.OutputEncoding = Encoding.UTF8;
        var console = AnsiConsole.Create(new AnsiConsoleSettings
        {
            Ansi        = AnsiSupport.Yes,
            Interactive = InteractionSupport.Yes,
        });

        var resources = new SharedResources()
        {
            Console  = console,
            GamePlan = gamePlan,
            Random   = random,
        };

        return new GameManager(
            resources,
            LetterPoolDisplay.Create(resources),
            new SubmissionManager
            {
                Scorer     = new ScrabbleScorer(),
                WordLookup = new WiktionaryClient(),
                SubmissionScreener = new LetterPoolSubmissionScreener()
                {
                    LetterPool        = resources.GamePlan.ProgenitorPool,
                    MinimumWordLength = resources.GamePlan.MinimumWordLength
                }
            }
        );
    }

    public void GameLoop()
    {
        while (true)
        {
            WriteFrame();
            ProcessFrame();
        }
    }

    private void ProcessFrame()
    {
        var rawUserInput = ConsoleHelpers.ReadNonEmptyInput();

        _previousSubmission = _submissionManager.SubmitRawInput(rawUserInput, _sharedResources.GamePlan.Language);
    }

    private void WriteFrame()
    {
        _sharedResources.Console.Clear();

        var letterPool = SpectreFactory.RenderLetterPool(
            _sharedResources.GamePlan.ProgenitorPool,
            _letterPoolDisplay.CurrentDisplay,
            _sharedResources
        );

        var submissions = SpectreFactory.RenderSubmissions(
            _submissionManager.GetCurrentSubmissions(),
            _submissionManager.GetCurrentJudgementHistory(),
            _sharedResources.GamePlan.Theme
        );

        _sharedResources.Console.Write(
            new Columns(letterPool, submissions)
            {
                Expand = false
            }
        );

        if (_previousSubmission is { IsT1: true })
        {
            _sharedResources.Console.WriteLine($"❌ {_previousSubmission.Value.AsT1.Reason}");
        }
    }
}