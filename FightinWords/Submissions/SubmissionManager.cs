using System.Collections.Immutable;
using FightinWords.Scoring;
using FightinWords.WordLookup;
using FightinWords.WordLookup.Wiktionary;
using JetBrains.Annotations;
using OneOf;

namespace FightinWords.Submissions;

public class SubmissionManager
{
    public enum Legality
    {
        /// <summary>
        /// The submission was accepted, but the <see cref="SubmissionManager.WordLookup"/> told us it wasn't a word.
        /// </summary>
        FakeWord,

        /// <summary>
        /// The submission was accepted and returned something from the <see cref="SubmissionManager.WordLookup"/>.
        /// </summary>
        RealWord,
    }

    public enum Freshness
    {
        Fresh,
        Stale,
    }

    public required IScorer Scorer { get; init; }

    public required IWordLookup WordLookup { get; init; }

    public required ISubmissionScreener<Word, Word> SubmissionScreener { get; init; }

    /// <summary>
    /// TODO: Having both this and <see cref="WiktionaryClient._responseCache"/> is redundant.
    /// </summary>
    private readonly Dictionary<LangWord, WordRating?> _submissions = new();

    /// <summary>
    /// The combined results from the <see cref="IScorer"/> and <see cref="IWordLookup"/> for a given word,
    /// which are always the same, regardless of the state of the game.
    /// </summary>
    public sealed record WordRating(int Points, ValueArray<WordDefinition> Definitions);

    private readonly History<Judgement> _judgementHistory = new(10);

    [MustUseReturnValue]
    public OneOf<Judgement, Failure> SubmitWord(Word word, Language language)
    {
        if (SubmissionScreener.ScreenInput(word).TryPickT0(out var screenedWord, out var failure))
        {
            var judgement = JudgeWord(screenedWord, language);
            _judgementHistory.Record(judgement);
            return judgement;
        }

        return failure;
    }

    [MustUseReturnValue]
    private Judgement JudgeWord(Word word, Language language)
    {
        if (_submissions.TryGetValue(new LangWord(word, language), out var previousRating))
        {
            // Return a new judgement for a previously-submitted word
            var legality = previousRating is null ? Legality.FakeWord : Legality.RealWord;
            return new Judgement(word, legality, Freshness.Stale, previousRating);
        }

        // Perform a new judgement
        var newRating = CalculateWordRating(word, language);
        _submissions.Add(new LangWord(word, language), newRating);
        return new Judgement(word, newRating is null ? Legality.FakeWord : Legality.RealWord, Freshness.Fresh,
            newRating);
    }

    private WordRating? CalculateWordRating(Word word, Language language)
    {
        var definitions = WordLookup
                          .GetDefinitions(word.ToString(), language)
                          .ToImmutableArray();

        if (definitions.IsEmpty)
        {
            return null;
        }

        var points = Scorer.ComputeScore(word.ToString(), language);
        return new WordRating(points, definitions);
    }

    public ImmutableDictionary<LangWord, WordRating?> GetCurrentSubmissions()
    {
        return _submissions
               .OrderBy(it => it.Key.Word.ToString())
               .ToImmutableDictionary();
    }

    public ImmutableArray<Judgement> GetCurrentJudgementHistory() => _judgementHistory.ToImmutableArray();
}