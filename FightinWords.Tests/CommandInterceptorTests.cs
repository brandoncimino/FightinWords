using FightinWords.Console;
using FluentAssertions;

namespace FightinWords;

public class CommandInterceptorTests
{
    [Test]
    public void CommandInterceptorTest(
        [Values("@", "", "//")] string prefix
    )
    {
        var interceptor = new CommandInterceptor<DayOfWeek>(
            new Dictionary<DayOfWeek, IEnumerable<string>>()
            {
                [DayOfWeek.Monday]    = [],
                [DayOfWeek.Wednesday] = ["humpday"]
            }
        )
        {
            Prefix = prefix
        };

        var intercepted = interceptor.ScreenInput(prefix + "H");

        intercepted.AsT0
                   .Should()
                   .Be(DayOfWeek.Wednesday);
    }

    [Test]
    public void RejectsUnknownCommands(
        [Values("@", "", "//")] string prefix
    )
    {
        var interceptor = new CommandInterceptor<DayOfWeek>(
            new Dictionary<DayOfWeek, IEnumerable<string>>()
            {
                [DayOfWeek.Monday]    = [],
                [DayOfWeek.Wednesday] = ["humpday"]
            }
        )
        {
            Prefix = prefix
        };

        var actualResult = interceptor.ScreenInput(prefix + "yolo");
        TestHelpers.AssertEquals(actualResult.IsT1, true);
    }
}