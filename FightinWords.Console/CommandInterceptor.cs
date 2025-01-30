using FightinWords.Submissions;
using OneOf;

namespace FightinWords.Console;

/// <summary>
/// Matches user input to the most appropriate <typeparamref name="COMMAND"/>.
/// </summary>
public sealed class CommandInterceptor<COMMAND> : ISubmissionScreener<string, COMMAND?, Failure>
    where COMMAND : struct, Enum
{
    /// <summary>
    /// The first thing you type that indicates that the following input is a special command.
    /// Usually <c>!</c>, <c>/</c>, <c>-</c>, or <c>--</c>.
    /// </summary>
    public string Prefix { get; init; } = "";

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

        return match.Map(
            alias => (COMMAND?)Enum.Parse<COMMAND>(alias.CanonicalName),
            failMessage => new Failure(failMessage)
        );
    }
}