using System.Collections.Immutable;
using FightinWords.Submissions;

namespace FightinWords.Console;

public record FinalResults(
    ImmutableDictionary<LangWord, SubmissionManager.WordRating?> Submissions
)
{
    public int TotalScore => Submissions.Sum(it => it.Value?.Points ?? 0);
}