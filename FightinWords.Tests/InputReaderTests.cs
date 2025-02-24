using System.Collections.Immutable;
using FightinWords.Console;
using FightinWords.Submissions;
using OneOf;

namespace FightinWords;

public class InputReaderTests
{
    public record Scenario(string RawInput, OneOf<Command, Word, Failure> ExpectedParsed);

    private static readonly CommandInterceptor<Command> _commandInterceptor =
        new(ImmutableDictionary<Command, IEnumerable<string>>.Empty);

    public static IEnumerable<Scenario> GetScenarios()
    {
        return
        [
            new Scenario("yolo swaggins",          Word.Parse("yoloswaggins")),
            new Scenario(Command.Start.ToString(), Command.Start)
        ];
    }

    [Test]
    public void InputReader_PrefersCommands()
    {
        var expectedCommand = new CommandLine<Command>(Command.Start, []);

        var reader = new InputReader()
        {
            CommandInterceptor = _ => expectedCommand,
            WordParser         = _ => throw TestHelpers.Fail()
        };

        var actualResult = reader.ParseUserInput("yolo");
        TestHelpers.AssertEquals(actualResult.RawInput,             "yolo");
        TestHelpers.AssertEquals(actualResult.Parsed,               expectedCommand);
        TestHelpers.AssertEquals(reader.GetInputHistory().Single(), actualResult);
    }

    [Test]
    public void InputReader_WhenCommandFails_ThenImmediatelyReturn()
    {
        var expectedFailure = new Failure("so bad");
        var reader = new InputReader()
        {
            CommandInterceptor = _ => expectedFailure,
            WordParser         = _ => throw TestHelpers.Fail()
        };

        var actualResult = reader.ParseUserInput("yolo");
        TestHelpers.AssertEquals(actualResult.RawInput,             "yolo");
        TestHelpers.AssertEquals(actualResult.Parsed,               expectedFailure);
        TestHelpers.AssertEquals(reader.GetInputHistory().Single(), actualResult);
    }

    [Test]
    public void InputReader_WhenCommandReturnsNull_ThenParserIsInvoked_Success()
    {
        var expectedWord = Word.Parse("success");
        var reader = new InputReader()
        {
            CommandInterceptor = _ => null,
            WordParser         = _ => expectedWord
        };

        var actualResult = reader.ParseUserInput("yolo");
        TestHelpers.AssertEquals(actualResult.RawInput,             "yolo");
        TestHelpers.AssertEquals(actualResult.Parsed,               expectedWord);
        TestHelpers.AssertEquals(reader.GetInputHistory().Single(), actualResult);
    }

    [Test]
    public void InputReader_WhenCommandReturnsNull_ThenParserIsInvoked_Failure()
    {
        var expectedFailure = new Failure("disgusting");
        var reader = new InputReader()
        {
            CommandInterceptor = _ => null,
            WordParser         = _ => expectedFailure
        };

        var actualResult = reader.ParseUserInput("yolo");
        TestHelpers.AssertEquals(actualResult.RawInput,             "yolo");
        TestHelpers.AssertEquals(actualResult.Parsed,               expectedFailure);
        TestHelpers.AssertEquals(reader.GetInputHistory().Single(), actualResult);
    }
}