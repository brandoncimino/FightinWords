namespace FightinWords.Console;

public interface IPhaseManager<
    COMMAND
> where COMMAND : struct, Enum
{
    // GamePlanner
    //  Command: `Command`

    // GameReferee
    //  Command:

    public InputReader<COMMAND>                     InputReader     { get; }
    public Func<CommandLine<COMMAND>, UserFeedback> CommandExecutor { get; }
    public Func<Word, UserFeedback>                 WordProcessor   { get; }

    public sealed UserFeedback ProcessUserInput(string rawUserInput)
    {
        var inputOrCommand = InputReader.ParseUserInput(rawUserInput);

        return inputOrCommand.Parsed.Switch(
            command => CommandExecutor(command),
            word => WordProcessor(word),
            failure => UserFeedback.Error(failure)
        );
    }
}

public class InGameManager : IPhaseManager<GameReferee.SortCommand>
{
    public GameReferee GameReferee { get; init; }

    public InputReader<GameReferee.SortCommand>                     InputReader     { get; init; }
    public Func<CommandLine<GameReferee.SortCommand>, UserFeedback> CommandExecutor { get; init; }
    public Func<Word, UserFeedback>                                 WordProcessor   => GameReferee.JudgeWord;

    private UserFeedback ExecuteCommand(CommandLine<GameReferee.SortCommand> commandLine)
    {
        GameAssistantDirector.TrySort(commandLine.Arguments,)
    }

    public string ReadUserInput(string input)
    {
        InputReader
    }
}