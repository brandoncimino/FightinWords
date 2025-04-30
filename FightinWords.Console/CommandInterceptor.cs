using FightinWords.Submissions;
using OneOf;

namespace FightinWords.Console;

/// <summary>
/// Matches user input to the most appropriate <typeparamref name="COMMAND"/>.
/// </summary>
public sealed class CommandInterceptor<COMMAND> : ISubmissionScreener<string, CommandLine<COMMAND>?>
    where COMMAND : struct, Enum
{
    /// <summary>
    /// The first thing you type that indicates that the following input is a special command.
    /// Usually <c>!</c>, <c>/</c>, <c>-</c>, or <c>--</c>.
    /// </summary>
    public string Prefix { get; init; } = "!";

    public UnknownCommandBehavior UnknownCommandBehavior { get; init; } = UnknownCommandBehavior.Reject;

    private readonly AliasMatcher _commandMatcher;

    public CommandInterceptor(IDictionary<COMMAND, IEnumerable<string>> aliases) : this(new AliasMatcher(
        aliases.Select(it =>
            new AliasMatcher.KnownAlias(
                Enum.GetName(it.Key)!,
                [..it.Value]
            )
        )
    ))
    {
    }

    private CommandInterceptor(AliasMatcher aliasMatcher)
    {
        _commandMatcher = aliasMatcher;
    }

    /// <summary>
    /// Creates a <see cref="CommandInterceptor{COMMAND}"/> backed by <see cref="AliasMatcher.ForEnum{COMMAND}"/>.
    /// </summary>
    public CommandInterceptor()
    {
        _commandMatcher = AliasMatcher.ForEnum<COMMAND>();
    }

    public OneOf<CommandLine<COMMAND>?, Failure> ScreenInput(string rawSubmission)
    {
        var splitSubmission =
            rawSubmission.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (splitSubmission is [var command, .. var args])
        {
            var commandAlone = ScreenCommandAlone(command);
            return commandAlone switch
            {
                { IsT0: true, AsT0 : { } cmd }     => new CommandLine<COMMAND>(cmd, args.ToValueArray()),
                { IsT0: true, AsT0 : null }        => null,
                { IsT0: false, AsT1: var failure } => failure
            };
        }

        throw new ArgumentException(
            $"The {nameof(rawSubmission)} argument was empty and/or whitespace! (`{rawSubmission}`)");
    }

    private OneOf<COMMAND?, Failure> ScreenCommandAlone(string rawSubmission)
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
            UnknownCommandBehavior.Reject => OneOf<COMMAND?, Failure>.FromT1(failure.GetMessage()),
            _ => throw new ArgumentOutOfRangeException(
                $"Unkown {nameof(UnknownCommandBehavior)}: {UnknownCommandBehavior}")
        };
    }
}

public readonly record struct CommandLine<COMMAND>(COMMAND Command, ValueArray<string> Arguments) where COMMAND : struct, Enum;