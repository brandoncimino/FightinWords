using FightinWords.WordLookup;

namespace FightinWords.Console;

/// <summary>
/// The stuff that needs to get configured before we can start playing, and shouldn't change while in-game.
/// </summary>
public sealed record GamePlan
{
    public required Language             Language          { get; init; }
    public required Word                 ProgenitorPool    { get; init; }
    public required TimeSpan             TimeLimit         { get; init; }
    public          SpectreFactory.Theme Theme             { get; init; } = new();
    public          int                  MinimumWordLength { get; init; } = 3;
}