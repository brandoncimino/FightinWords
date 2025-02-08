using FightinWords.Console.Rendering;
using FightinWords.Submissions;
using Spectre.Console;

namespace FightinWords.Console;

/// <summary>
/// Stuff that, <i><b>during a game</b></i>, different "components" <i>(<see cref="LetterPoolDisplay"/>, <see cref="SubmissionManager"/>, etc.)</i>
/// should be sharing, even though they don't know it, such as a <see cref="Random"/> number generator.
/// </summary>
/// <remarks>
/// Note that these things are expected to be "read-only" - the purpose of this class is <b>coordination</b>, not communication. 
/// </remarks>
public sealed class SharedResources
{
    public required GamePlan     GamePlan { get; init; }
    public required Random       Random   { get; init; }
    public required IAnsiConsole Console  { get; init; }

    public static SharedResources Create(
        GamePlan     gamePlan,
        IAnsiConsole console
    )
    {
        GamePlan.Validate(gamePlan);

        var random = gamePlan.RandomSeed switch
        {
            { Length: > 0 } => new Random(ToIntegerSeed(gamePlan.RandomSeed)),
            _               => Random.Shared
        };

        return new SharedResources
        {
            Console  = console,
            GamePlan = gamePlan,
            Random   = random,
        };

        static int ToIntegerSeed(string seed)
        {
            // TODO: There's probably a consistent hash code function or something that's much more useful than this 🤷‍♀️
            return seed.Sum(static it => it);
        }
    }
}