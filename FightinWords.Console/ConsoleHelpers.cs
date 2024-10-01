using JetBrains.Annotations;

namespace FightinWords.Console;

public static class ConsoleHelpers
{
    [MustUseReturnValue]
    public static string ReadNonEmptyInput()
    {
        string? userInput         = default;
        var (beforeLeft, beforeTop) = System.Console.GetCursorPosition();
        while (string.IsNullOrEmpty(userInput))
        {
            System.Console.SetCursorPosition(beforeLeft, beforeTop);
            userInput = System.Console.ReadLine();
        }

        return userInput;
    }
}