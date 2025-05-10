using System.Diagnostics;
using System.Diagnostics.Contracts;
using FightinWords.Console.Rendering;
using FightinWords.Submissions;
using OneOf;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace FightinWords.Console;

/// <summary>
/// Responsible for setting up the <see cref="GamePlan"/>, transitioning the <see cref="_gameState"/>, etc. - basically, "out-of-game" stuff.
///<p/>
/// This is in contrast to the <see cref="GameReferee"/>, who issues rulings, scores, etc.
/// </summary>
public sealed class GameDirector
{
    private readonly InputReader<Command>            _inputReader = new();
    private          OneOf<GamePlanner, GameReferee> _gameState   = new GamePlanner();
    public required  IAnsiConsole                    Console { get; init; }
    public           SpectreFactory.Theme            Theme   { get; init; } = new();

    private UserFeedback? _userFeedback;

    public GameDirector()
    {
        _userFeedback = _gameState.AsT0.LetterPoolConfigScreener.GetPrompt();
    }

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
        Console.Clear();
        Console.Write(RenderFrame());

        var userInput = _inputReader.ReadUserInput();

        if (ProcessUserInput(userInput).TryPickT0(out var feedback, out var finalResults))
        {
            _userFeedback = feedback;
        }

        return finalResults;
    }

    private OneOf<UserFeedback, FinalResults> ProcessUserInput(InputReader<Command>.UserInput userInput)
    {
        Debug.WriteLine($"Processing user input: {userInput} // {userInput.Parsed}");

        var result = userInput.Parsed.Switch(
            ExecuteCommand,
            it => HandleWordInput(it),
            failure =>
            {
                Debug.WriteLine($"Passing along failure from parser: {failure}");
                return failure;
            });

        Debug.WriteLine($"Processed result: {result}");

        return result.Switch(
            OneOf<UserFeedback, FinalResults>.FromT0,
            OneOf<UserFeedback, FinalResults>.FromT1,
            failure => UserFeedback.Error(failure)
        );
    }

    [Pure]
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
        Debug.WriteLine($"Handling user word input: {userInput} (Current state: {_gameState})");
        if (_gameState.TryPickT0(out var planner, out var referee))
        {
            Debug.WriteLine("Configuring letter pool...");
            return planner.TryConfigureLetterPool(userInput)
                          .Match(
                              _ => TryStartGame(),
                              failure => failure
                          );
        }

        return referee.JudgeWord(userInput);
    }

    private static readonly AliasMatcher
        LetterSortingMatcher = AliasMatcher.ForEnum<LetterPoolDisplay.LetterSorting>();

    private OfThree<UserFeedback, FinalResults, Failure> ExecuteCommand(CommandLine<Command> command)
    {
        Debug.WriteLine($"Executing command: {command}");
        return command switch
        {
            { Command: Command.Start }                     => TryStartGame(),
            { Command: Command.Exit }                      => TryEndGame(),
            { Command: Command.Sort, Arguments: var args } => TrySort(args),
            _                                              => throw new UnreachableException()
        };
    }

    private OneOf<UserFeedback, Failure> TrySort(ValueArray<string> args)
    {
        return _gameState.Match(
            planner => new Failure("Nothing to sort at the moment; you aren't in-game."),
            referee => GameAssistantDirector.TrySort(args, referee, LetterSortingMatcher)
        );
    }
}