using System.Text;
using FightinWords.Scoring;
using FightinWords.Submissions;
using FightinWords.WordLookup;
using FluentAssertions;

namespace FightinWords;

public class SubmissionManagerTests
{
    private sealed class LengthScorer : IScorer
    {
        public int ComputeScore(ReadOnlySpan<Rune> word, Language language)
        {
            return word.Length;
        }

        public int ComputeScore(SpanRuneEnumerator word, Language language)
        {
            var score = 0;

            while (word.MoveNext())
            {
                score += 1;
            }

            return score;
        }
    }

    private static SubmissionManager CreateTestManager()
    {
        var lookup = DictionaryWordLookup.Empty
                                         .WithDefinition(new WordDefinition("yolo", Language.English, PartOfSpeech.Noun,
                                             """Acronym of "You Oughtta Look Out"."""))
                                         .WithDefinition(new WordDefinition("swag", Language.English, PartOfSpeech.Noun,
                                             """Abbreviation of "swagger"."""))
                                         .WithDefinition(new WordDefinition("swagger", Language.English,
                                             PartOfSpeech.Noun, "A bold or arrogant strut."))
                                         .WithDefinition(new WordDefinition("swagger", Language.English,
                                             PartOfSpeech.Verb,
                                             "To behave (especially to walk or carry oneself) in a pompous, superior manner."));

        var scorer = new LengthScorer();
        var screener = new LetterPoolSubmissionScreener()
        {
            LetterPool        = Word.Parse("yolo"),
            MinimumWordLength = 3
        };
        return new SubmissionManager()
        {
            WordLookup         = lookup,
            Scorer             = scorer,
            SubmissionScreener = screener
        };
    }

    [Test]
    public void ValidWord()
    {
        var definition = new WordDefinition("yolo", Language.English, PartOfSpeech.Noun, "You Oughtta Look Out");
        var lookup     = DictionaryWordLookup.Empty.WithDefinition(definition);
        var scorer     = new LengthScorer();
        var screener = new LetterPoolSubmissionScreener()
        {
            LetterPool        = Word.Parse("yolo"),
            MinimumWordLength = 3
        };

        var submissionManager = new SubmissionManager()
        {
            WordLookup         = lookup,
            Scorer             = scorer,
            SubmissionScreener = screener
        };

        var submissionResult = submissionManager.SubmitWord(Word.Parse(definition.Word), definition.Language);

        var points = scorer.ComputeScore(definition.Word, definition.Language);

        var expectedRating = new SubmissionManager.WordRating(points, new ValueArray<WordDefinition>([definition]));
        TestHelpers.AssertEquals(
            submissionResult,
            new Judgement(
                Word.Parse(definition.Word),
                SubmissionManager.Legality.RealWord,
                SubmissionManager.Freshness.Fresh,
                expectedRating
            )
        );

        var secondSubmission = submissionManager.SubmitWord(Word.Parse(definition.Word), definition.Language);
        TestHelpers.AssertEquals(
            secondSubmission,
            new Judgement(
                Word.Parse(definition.Word),
                SubmissionManager.Legality.RealWord,
                SubmissionManager.Freshness.Stale,
                expectedRating
            )
        );
    }

    // TODO: This test was relevant when the SubmissionManager accepted `string`s, not `Word`s.
    // [Test]
    // public void InvalidWord()
    // {
    //     var manager = CreateTestManager();
    //     var nonWord = Guid.NewGuid().ToString();
    //
    //     var result = manager.SubmitRawInput(nonWord, Language.English);
    //
    //     TestHelpers.AssertEquals(
    //         result,
    //         new Judgement(
    //             Word.Parse(nonWord),
    //             SubmissionManager.Legality.FakeWord,
    //             SubmissionManager.Freshness.Fresh,
    //             null
    //         )
    //     );
    //
    //     var secondResult = manager.SubmitRawInput(nonWord, Language.English);
    //
    //     TestHelpers.AssertEquals(
    //         secondResult,
    //         new Judgement(
    //             Word.Parse(nonWord),
    //             SubmissionManager.Legality.FakeWord,
    //             SubmissionManager.Freshness.Stale,
    //             null
    //         )
    //     );
    // }

    [Test]
    public void FailedScreening()
    {
        var manager = CreateTestManager();

        var firstResult = manager.SubmitWord(Word.Parse("yy"), Language.English);
        firstResult.IsT1.Should().BeTrue();
    }

    [Test]
    public void ValidWord_wrongLanguage()
    {
        var manager = CreateTestManager();
        var word    = Word.Parse("yolo");

        Assert.Throws<ArgumentException>(() => manager.SubmitWord(word, Language.Gothic));
        // var firstResult = manager.SubmitWord(word, Language.Gothic);
        // TestHelpers.AssertEquals(
        //     firstResult,
        //     new Judgement(
        //         word,
        //         SubmissionManager.Legality.FakeWord,
        //         SubmissionManager.Freshness.Fresh,
        //         null
        //     )
        // );

        // var secondResult = manager.SubmitWord(word, Language.Gothic);
        // TestHelpers.AssertEquals(
        //     secondResult,
        //     new Judgement(
        //         word,
        //         SubmissionManager.Legality.FakeWord,
        //         SubmissionManager.Freshness.Stale,
        //         null
        //     )
        // );
    }

    [Test]
    public void LangWordEquality()
    {
        var word1 = Word.Parse("yolo");
        var word2 = Word.Parse("yolo");
        word1.Equals(word2).Print();
        TestHelpers.AssertEquals(word1, word2);

        var langWord1 = new LangWord(Word.Parse("yolo"), Language.English);
        var langWord2 = new LangWord(Word.Parse("yolo"), Language.English);
        langWord1.Equals(langWord2).Print();
        TestHelpers.AssertEquals(langWord1, langWord2);
    }
}