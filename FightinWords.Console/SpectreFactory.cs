using FightinWords.Console.Rendering;
using FightinWords.Submissions;
using FightinWords.WordLookup;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace FightinWords.Console;

public static class SpectreFactory
{
    public readonly record struct Theme()
    {
        public Style LetterBase { get; init; } = new(decoration: Decoration.Bold);
        public Style Vowel      { get; init; } = Color.Blue;
        public Style Semivowel  { get; init; } = Color.Aqua;
        public Style Consonant  { get; init; } = Color.Green;
        public Style Points     { get; init; } = Color.Purple;

        public Style SubmissionBase { get; init; } = Style.Plain;
        public Style BadSubmission  { get; init; } = Color.DarkRed;
        public Style GoodSubmission { get; init; } = Color.Green;
        public Style PartOfSpeech   { get; init; } = new(decoration: Decoration.Italic);

        public Decoration HighlightDecoration { get; init; } =
            Decoration.Bold | Decoration.Italic | Decoration.Underline;

        public Icons Icons { get; init; } = new();

        public Style NeutralFeedback  { get; init; } = Style.Plain;
        public Style PositiveFeedback { get; init; } = Color.Green;
        public Style NegativeFeedback { get; init; } = Color.DarkRed;

        public Style GetLetterStyle(Phonology phonology)
        {
            var phonologicalStyle = phonology switch
            {
                Phonology.Vowel     => Vowel,
                Phonology.Semivowel => Semivowel,
                Phonology.Consonant => Consonant,
                _                   => Style.Plain
            };

            return LetterBase.Combine(phonologicalStyle);
        }

        public Style GetSubmissionStyle(bool isGood, bool highlighted)
        {
            var style = SubmissionBase.Combine(isGood ? GoodSubmission : BadSubmission);
            if (highlighted)
            {
                style = style.Decoration(HighlightDecoration);
            }

            return style;
        }
    }

    public readonly record struct Icons()
    {
        public const string ErrorDefault     = "❌";
        public const string RejectionDefault = "👎";
        public const string SuccessDefault   = "✅";
        public const string StaleWordDefault = "🔁";

        public string Error     { get; init; } = ErrorDefault;
        public string Rejection { get; init; } = RejectionDefault;
        public string Success   { get; init; } = SuccessDefault;
        public string StaleWord { get; init; } = StaleWordDefault;
    }

    public static IRenderable RenderLetterPool(
        IEnumerable<Grapheme> asOriginallyGiven,
        IEnumerable<Grapheme> currentDisplayOrder,
        SharedResources       sharedResources
    )
    {
        var pg = new Paragraph();
        foreach (var it in asOriginallyGiven)
        {
            pg.Append(it.Source.EscapeMarkup(), sharedResources.GamePlan.Theme.LetterBase);
        }

        pg.Append("\n".EscapeMarkup());

        foreach (var it in currentDisplayOrder)
        {
            pg.Append(it.Source.EscapeMarkup(),
                sharedResources.GamePlan.Theme.GetLetterStyle(it.GetPhonology(sharedResources.GamePlan.Language)));
        }

        return new Panel(pg)
        {
            Border = BoxBorder.Rounded
        };
    }

    public static IRenderable RenderSubmissions(
        IDictionary<LangWord, SubmissionManager.WordRating?> submissions,
        IReadOnlyList<Judgement>                             judgementHistory,
        Theme                                                theme
    )
    {
        var mostRecent = judgementHistory.Count > 0 ? judgementHistory[0] : null;
        var goodCol = RenderSubmissionList(
            [..submissions.Where(it => it.Value is not null)],
            mostRecent?.Word,
            theme
        );

        var badCol = RenderSubmissionList(
            [..submissions.Where(it => it.Value is null)],
            mostRecent?.Word,
            theme
        );

        var table = new Table()
        {
            Border      = TableBorder.Rounded,
            ShowHeaders = false,
            Expand      = true
        };

        var mostRecentSuccess = MostRecentSuccess(judgementHistory);
        table.AddColumns("✅".EscapeMarkup(), "❌".EscapeMarkup(), "Previous");
        table.AddRow(new Rows(goodCol), new Rows(badCol), RenderPreviousJudgement(mostRecentSuccess, theme));
        return table;
    }

    public static Columns RenderScoreBoard(
        IDictionary<LangWord, SubmissionManager.WordRating?> submissions,
        IReadOnlyList<Judgement>                             judgementHistory,
        LetterPoolDisplay                                    letterPoolDisplay,
        SharedResources                                      sharedResources
    )
    {
        var letterPool = RenderLetterPool(
            sharedResources.GamePlan.ProgenitorPool,
            letterPoolDisplay.CurrentDisplay,
            sharedResources
        );

        var submissionDisplay = RenderSubmissions(
            submissions,
            judgementHistory,
            sharedResources.GamePlan.Theme
        );

        var cols = HandleNulls([letterPool, submissionDisplay], NullHandling.Skip);
        return new Columns(cols)
        {
            Expand = false
        };
    }

    private static Judgement? MostRecentSuccess(IReadOnlyList<Judgement> judgementHistory)
    {
        var mostRecentSuccess =
            judgementHistory.FirstOrDefault(it =>
            {
                ArgumentNullException.ThrowIfNull(it);
                return it.Legality == SubmissionManager.Legality.RealWord;
            });
        return mostRecentSuccess;
    }

    public static IRenderable RenderPreviousJudgement(Judgement? judgement, Theme theme)
    {
        return judgement switch
        {
            null => Text.Empty,
            { Score.Definitions: var definitions } => RenderDefinition(
                definitions.First(it => it is { Definition.Length: > 0 }), theme),
            _ => new Markup($"{judgement.Word} is not a real word".EscapeMarkup()),
        };
    }

    private static Columns RenderDefinition(WordDefinition definition, Theme theme)
    {
        return new Columns(
            Markup.FromInterpolated($"{definition.Word}",           Style.Plain),
            Markup.FromInterpolated($"({definition.PartOfSpeech})", theme.PartOfSpeech),
            new Markup(HtmlToSpectreMarkup.Convert(definition.Definition))
        );
    }

    private static IRenderable RenderSubmissionRow(
        SubmittedWord submission,
        bool          highlighted,
        Theme         theme
    )
    {
        if (submission.Rating is null)
        {
            return Markup.FromInterpolated($"{submission.Word}", theme.GetSubmissionStyle(false, highlighted));
        }

        return new Columns(
            Markup.FromInterpolated($"{submission.Word}", theme.GetSubmissionStyle(true, highlighted)),
            Markup.FromInterpolated($"{submission.Rating.Points}",
                highlighted ? theme.Points.Decoration(theme.HighlightDecoration) : theme.Points)
        );
    }

    private readonly record struct SubmittedWord(Word Word, Language Language, SubmissionManager.WordRating? Rating)
    {
        public static implicit operator SubmittedWord(KeyValuePair<LangWord, SubmissionManager.WordRating?> submission)
        {
            return new SubmittedWord(submission.Key.Word, submission.Key.Language, submission.Value);
        }
    }

    private static IEnumerable<IRenderable> RenderSubmissionList(
        IEnumerable<SubmittedWord> submissions,
        Word?                      previousSubmission,
        Theme                      theme
    )
    {
        return submissions.OrderBy(it => it.Word.Length)
                          .ThenBy(it => it.Word.ToString(), StringComparer.InvariantCulture)
                          .ThenBy(it => it.Rating?.Points)
                          .Select(it => RenderSubmissionRow(it, it.Word == previousSubmission, theme));
    }

    public enum NullHandling
    {
        Error,
        Skip,
        Empty,
    }

    public static IEnumerable<IRenderable> HandleNulls(
        this IEnumerable<IRenderable?> renderables,
        NullHandling                   nullHandling
    )
    {
        return (nullHandling switch
        {
            NullHandling.Error => renderables.Select(it => Preconditions.RequireNotNull(it)),
            NullHandling.Skip  => renderables.Where(it => it is not null),
            NullHandling.Empty => renderables.Select(it => it ?? Text.Empty),
            _                  => throw new ArgumentOutOfRangeException(nameof(nullHandling), nullHandling, null)
        })!;
    }
}