using System.Diagnostics;
using FightinWords.Submissions;
using OneOf;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace FightinWords.Console;

/// <summary>
/// Responsible for setting up the <see cref="GamePlan"/>, transitioning the <see cref="GamePhase"/>, etc. - basically, "out-of-game" stuff.
///<p/>
/// This is in contrast to the <see cref="GameReferee"/>, who issues rulings, scores, etc.
/// </summary>
public sealed class GameDirector
{
    private readonly InputReader                     _inputReader = new();
    private          OneOf<GamePlanner, GameReferee> _gameState;
    public required  IAnsiConsole                    Console { get; init; }
    public           SpectreFactory.Theme            Theme   { get; init; } = new();

    private UserFeedback? _userFeedback;

    #region Start & End

    private OneOf<UserFeedback, Failure> TryStartGame()
    {
        if (_gameState.TryPickT0(out var planner, out var referee))
        {
            var newReferee = GameAssistantDirector.StartGame(planner, Console, Random.Shared);
            _gameState = newReferee;
            return newReferee.GetPrompt();
        }

        return new Failure("Cannot start the game because it is already running.");
    }

    private OneOf<FinalResults, Failure> TryEndGame()
    {
        if (_gameState.TryPickT0(out var planner, out var referee))
        {
            return new Failure("Cannot end the game because it is not in progress.");
        }

        (_gameState, var results) = GameAssistantDirector.EndGame(referee);
        return results;
    }

    #endregion

    public FinalResults? GameLoop()
    {
        var userInput = _inputReader.ReadUserInput();

        if (ProcessUserInput(userInput).TryPickT0(out var feedback, out var finalResults))
        {
            _userFeedback = feedback;
        }

        return finalResults;
    }

    private OneOf<UserFeedback, FinalResults> ProcessUserInput(InputReader.UserInput userInput)
    {
        var result = userInput.Parsed.Switch(
            ExecuteCommand,
            it => HandleWordInput(it),
            failure => failure
        );

        return result.Switch(
            OneOf<UserFeedback, FinalResults>.FromT0,
            OneOf<UserFeedback, FinalResults>.FromT1,
            failure => UserFeedback.Error(failure, Theme)
        );
    }

    private IRenderable RenderFrame()
    {
        var body = _gameState.Match(
            planner => planner.ToRenderable(),
            referee => referee.ToRenderable()
        );

        return new Rows(
            SpectreFactory.HandleNulls([
                body,
                _userFeedback?.ToRenderable(Theme)
            ], SpectreFactory.NullHandling.Skip));
    }

    private OneOf<UserFeedback, Failure> HandleWordInput(Word userInput)
    {
        if (_gameState.TryPickT0(out var planner, out var referee))
        {
            return planner.TryConfigureLetterPool(userInput)
                          .Match(
                              _ => TryStartGame(),
                              failure => failure
                          );
        }

        return referee.JudgeWord(userInput);
    }

    private OfThree<UserFeedback, FinalResults, Failure> ExecuteCommand(Command command)
    {
        return command switch
        {
            Command.Start => TryStartGame(),
            Command.Exit  => TryEndGame(),
            _             => throw new UnreachableException()
        };
    }
}