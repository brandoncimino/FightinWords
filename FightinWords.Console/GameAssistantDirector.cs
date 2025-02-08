using Spectre.Console;

namespace FightinWords.Console;

/// <summary>
/// Static methods that implement bits of logic for the <see cref="GameDirector"/>.
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
}