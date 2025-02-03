using FightinWords.Submissions;
using OneOf;

namespace FightinWords.Console;

/// <summary>
/// Matches user input to the most appropriate <typeparamref name="COMMAND"/>.
/// </summary>
public sealed class CommandInterceptor<COMMAND> : ISubmissionScreener<string, COMMAND?>
    where COMMAND : struct, Enum
{
    /// <summary>
    /// The first thing you type that indicates that the following input is a special command.
    /// Usually <c>!</c>, <c>/</c>, <c>-</c>, or <c>--</c>.
    /// </summary>
    public string Prefix { get; init; } = "!";

    public UnknownCommandBehavior UnknownCommandBehavior { get; init; } = UnknownCommandBehavior.Reject;

    private readonly AliasMatcher _commandMatcher;

    public CommandInterceptor(IDictionary<COMMAND, IEnumerable<string>> aliases)
    {
        _commandMatcher = new AliasMatcher(
            aliases.Select(it =>
                new AliasMatcher.KnownAlias(
                    Enum.GetName(it.Key)!,
                    [..it.Value]
                )
            )
        );
    }

    public OneOf<COMMAND?, Failure> ScreenInput(string rawSubmission)
    {
        rawSubmission = rawSubmission.Trim().ToLowerInvariant();

        if (rawSubmission.StartsWith(Prefix) is false)
        {
            return null;
        }

        var trimmedUserInput = rawSubmission.AsSpan()[Prefix.Length..];

        var match = _commandMatcher.FindMatch(trimmedUserInput);

        if (match.TryPickT0(out var alias, out var failure))
        {
            return Enum.Parse<COMMAND>(alias.CanonicalName);
        }

        return UnknownCommandBehavior switch
        {
            UnknownCommandBehavior.Ignore => OneOf<COMMAND?, Failure>.FromT0(null),
            UnknownCommandBehavior.Reject => OneOf<COMMAND?, Failure>.FromT1(failure),
            _ => throw new ArgumentOutOfRangeException(
                $"Unkown {nameof(UnknownCommandBehavior)}: {UnknownCommandBehavior}")
        };
    }
}