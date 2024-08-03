using FightinWords.Scoring;
using FightinWords.WordLookup;

namespace FightinWords.Console;

public sealed class GameManager
{
    public string LetterPool { get; }
    public IWordLookup WordLookup { get; }
    public IScorer Scorer { get; }
    public Language Language { get; }

    public int SubmitWord(string word)
    {
        var score = Scorer.ComputeScore(word, Language);
        throw new NotImplementedException();
    }
}