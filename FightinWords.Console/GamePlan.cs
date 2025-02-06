using FightinWords.WordLookup;

namespace FightinWords.Console;

/// <summary>
/// The stuff that needs to get configured before we can start playing, and shouldn't change while in-game.
/// </summary>
public sealed record GamePlan
{
    public required Language             Language          { get; init; }
    public required Word                 ProgenitorPool    { get; init; }
    public required TimeSpan?            TimeLimit         { get; init; } = null;
    public          SpectreFactory.Theme Theme             { get; init; } = new();
    public          int                  MinimumWordLength { get; init; } = 3;
    public          string?              RandomSeed        { get; init; } = null;

    public static void Validate(GamePlan gamePlan)
    {
        Preconditions.Require(gamePlan.ProgenitorPool.Length > 0);
        Preconditions.Require(gamePlan.TimeLimit is null || gamePlan.TimeLimit > TimeSpan.Zero);
        Preconditions.Require(gamePlan.MinimumWordLength >= 0);
    }
}