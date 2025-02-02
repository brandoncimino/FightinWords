using System.Collections.Immutable;
using FightinWords.Submissions;
using OneOf;

namespace FightinWords.Console;

public class InputReader
{
    private readonly History<UserInput> _inputHistory = new(5);

    public record UserInput(
        string                          RawInput,
        OfThree<Command, Word, Failure> Parsed
    );

    public Screener<string, Command?> CommandInterceptor { get; init; } = new CommandInterceptor<Command>(
        new Dictionary<Command, IEnumerable<string>>()
        {
            [Command.Exit]  = ["quit"],
            [Command.Start] = []
        }).ScreenInput;

    public Screener<string, Word> WordParser { get; init; } = ParseWord;

    public UserInput ReadUserInput()
    {
        var rawInput = ConsoleHelpers.ReadNonEmptyInput();
        return ParseUserInput(rawInput);
    }

    public UserInput ParseUserInput(string rawInput)
    {
        var maybeCommand = CommandInterceptor(rawInput);
        var userInput = new UserInput(
            rawInput,
            maybeCommand switch
            {
                { IsT0: true, AsT0: { } command } => command,
                { IsT1: true }                    => maybeCommand.AsT1,
                _                                 => WordParser(rawInput)
            });

        _inputHistory.Record(userInput);
        return userInput;
    }

    private static OneOf<Word, Failure> ParseWord(string userInput)
    {
        if (Word.TryParse(userInput, out var word))
        {
            return word;
        }

        return new Failure("You entered some weird junk that we couldn't parse 🤷‍♀️");
    }

    public ImmutableList<UserInput> GetInputHistory()
    {
        return _inputHistory.ToImmutableList();
    }
}