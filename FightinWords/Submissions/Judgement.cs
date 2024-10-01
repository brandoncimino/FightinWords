namespace FightinWords.Submissions;

/// <summary>
/// The response given to the player for their submission, which depends on the current state of the game.
/// </summary>
public sealed record Judgement(
    Word                          Word,
    SubmissionManager.Legality    Legality,
    SubmissionManager.Freshness   Freshness,
    SubmissionManager.WordRating? Score);