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

    public const int DefaultMinimumWordLength = 4;
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
            LetterPool = pool;
        }

        return failure;
    }

    private static Word ResolveLetterPool(ILetterPool? letterPool, Random random)
    {
        return (letterPool ?? new RandomLetterPool(DefaultPoolSize))
            .LockIn(random);
    }

    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
    public IRenderable ToRenderable()
    {
        return new Text("This is the game");
    }
}