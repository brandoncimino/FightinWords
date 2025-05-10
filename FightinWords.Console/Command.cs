using System.Runtime.CompilerServices;
using System.Text.Json;

namespace FightinWords.Console;

public enum Command
{
    Exit,
    Start,
    Sort,
}

public readonly record struct CommandInvocation<COMMAND, ARGS>(COMMAND Command, CommandArgs<ARGS> Args)
    where ARGS : ITuple;

public sealed class CommandArgs<ARGS> where ARGS : ITuple
{
    public ARGS               Parsed { get; private init; }
    public ValueArray<string> Raw    { get; private init; }

    private CommandArgs(ValueArray<string> raw, ARGS parsed)
    {
        Parsed = parsed;
        Raw    = raw;
    }

    public static CommandArgs<ARGS> Parse(IEnumerable<string> rawArgs)
    {
        var rawArgsArray = rawArgs.ToValueArray();
        return new CommandArgs<ARGS>(
            rawArgsArray,
            CommandHelpers.ParseArgs<ARGS>(rawArgsArray.Values.AsSpan())
        );
    }
}

file static class CommandHelpers
{
    private static object?[] ParseArgs(ReadOnlySpan<string> rawArgs, ReadOnlySpan<Type> argTypes)
    {
        Preconditions.Require(rawArgs.Length, argTypes.Length);

        var parsed = new object?[rawArgs.Length];

        for (int i = 0; i < rawArgs.Length; i++)
        {
            parsed[i] = JsonSerializer.Deserialize(rawArgs[i], argTypes[i]);
        }

        return parsed;
    }

    public static ARGS ParseArgs<ARGS>(ReadOnlySpan<string> rawArgs) where ARGS : ITuple
    {
        var tupleType = typeof(ARGS);
        var tupleArgs = tupleType.GenericTypeArguments;
        Preconditions.Require(rawArgs.Length, tupleArgs.Length);

        var constructor = tupleType.GetConstructor(tupleArgs)!;
        var parsedArgs  = ParseArgs(rawArgs, tupleArgs);
        return (ARGS)constructor.Invoke(parsedArgs);
    }
}