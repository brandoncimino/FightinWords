using FightinWords.Submissions;
using Spectre.Console;

namespace FightinWords.Console;

/// <summary>
/// Stuff that different "components" <i>(<see cref="LetterPoolDisplay"/>, <see cref="SubmissionManager"/>, etc.)</i>
/// should be sharing, even though they don't know it, such as a <see cref="Random"/> number generator.
/// </summary>
/// <remarks>
/// Note that these things are expected to be "read-only" - the purpose of this class is <b>coordination</b>, not communication. 
/// </remarks>
public sealed class SharedResources
{
    public required GamePlan             GamePlan { get; init; }
    public required Random               Random   { get; init; }
    public required IAnsiConsole         Console  { get; init; }
}