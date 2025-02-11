using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FightinWords.Console.LetterPools;
using FightinWords.Submissions;
using FightinWords.WordLookup;
using OneOf;
using OneOf.Types;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace FightinWords.Console;

/// <summary>
/// The stateful "builder" that produces <see cref="GamePlan"/>s.
/// </summary>
/// <remarks>
/// The <see cref="GameDirector"/> delegates to this while the user is setting up the game.</remarks>
public class GamePlanner
{
    public LetterPoolConfigScreener LetterPoolConfigScreener { get; init; } = new();

    public ILetterPool? LetterPool        { get; set; }
    public int?         MinimumWordLength { get; set; }

    public const int DefaultMinimumWordLength = 3;
    public const int DefaultPoolSize          = 6;

    public GamePlan LockIn(Random random)
    {
        return new GamePlan
        {
            Language          = Language.English,
            ProgenitorPool    = ResolveLetterPool(LetterPool, random),
            TimeLimit         = null,
            MinimumWordLength = MinimumWordLength ?? DefaultMinimumWordLength,
        };
    }

    public OneOf<Success, Failure> TryConfigureLetterPool(Word userInput)
    {
        if (LetterPoolConfigScreener.ScreenInput(userInput).TryPickT0(out var pool, out var failure))
        {
            Debug.WriteLine($"Configured letter pool: {pool}");
            LetterPool = pool;
            return default(Success);
        }
        else
        {
            Debug.WriteLine($"Failed to configure letter pool: {failure}");
            return failure;
        }
    }

    private static Word ResolveLetterPool(ILetterPool? letterPool, Random random)
    {
        return (letterPool ?? new RandomLetterPool { PoolSize = DefaultPoolSize })
            .LockIn(random);
    }

    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
    public IRenderable ToRenderable()
    {
        var table = new Table()
        {
            Border = TableBorder.None
        };

        table.AddColumns("", "");
        table.AddRow("Minimum Word Length", (MinimumWordLength ?? DefaultMinimumWordLength).ToString());
        return table;
    }
}