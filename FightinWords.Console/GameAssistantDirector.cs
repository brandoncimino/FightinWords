using FightinWords.Console.Rendering;
using FightinWords.Submissions;
using OneOf;
using Spectre.Console;

namespace FightinWords.Console;

/// <summary>
/// Static methods that do actual work which the <see cref="GameDirector"/> takes credit for.
/// </summary>
public static class GameAssistantDirector
{
    public static GameReferee StartGame(GamePlanner planner, IAnsiConsole console, Random random)
    {
        var gamePlan        = planner.LockIn(random);
        var sharedResources = SharedResources.Create(gamePlan, console);
        var referee         = GameReferee.StartGame(sharedResources);
        return referee;
    }

    public static (GamePlanner, FinalResults) EndGame(GameReferee referee)
    {
        return (new GamePlanner(), new FinalResults(referee.GetCurrentSubmissions()));
    }

    public static OneOf<UserFeedback, Failure> TrySort(ValueArray<string> args, GameReferee referee, AliasMatcher letterSortingMatcher)
    {
        return args switch
        {
            []            => referee.Sort(GameReferee.SortCommand.SortNext),
            [var sortArg] => SortSingle(sortArg, referee, letterSortingMatcher),
            [_, _, ..]    => new Failure($"Expected exactly 1 argument, but got {args.Values.Length}: {args}")
        };

        static OneOf<UserFeedback, Failure> SortSingle(
            string       sortArg,
            GameReferee  referee,
            AliasMatcher letterSortingMatcher
        )
        {
            return letterSortingMatcher.FindMatch(sortArg)
                                       .Map(
                                           alias => Enum.Parse<GameReferee.SortCommand>(alias.CanonicalName),
                                           aliasFailure => new Failure(aliasFailure.GetMessage())
                                       )
                                       .MapT0(referee.Sort);
        }
    }
}