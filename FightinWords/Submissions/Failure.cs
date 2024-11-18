namespace FightinWords.Submissions;

public readonly record struct Failure(string Reason)
{
    public static implicit operator Failure(string reason) => new(reason);
}