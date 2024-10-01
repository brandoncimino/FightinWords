using System.Text.RegularExpressions;
using FightinWords.Submissions;
using FightinWords.WordLookup;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace FightinWords.Console;

public static partial class SpectreFactory
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
            HtmlToMarkup(definition.Definition)
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

    [GeneratedRegex("""<\s*a.*?href\s*?=\s*?"(?<dest>.*?)".*?>(?<text>.*?)</a>""")]
    private static partial Regex HyperlinkRegex();

    [GeneratedRegex("""<\s*([ib]).*?>(.*)<\s*/\s*\1\s*>""")]
    private static partial Regex BoldItalicRegex();

    [GeneratedRegex("""</?\s*span.*?\s*/?\s*>""")]
    private static partial Regex SpanRegex();

    private static Markup HtmlToMarkup(string html)
    {
        var markupEscaped       = html.EscapeMarkup();
        var linksReplaced       = HyperlinkRegex().Replace(markupEscaped, @"[link=$1]$2[/]");
        var boldItalicsReplaced = BoldItalicRegex().Replace(linksReplaced, @"[$1]$2[/]");
        var spansRemoved        = SpanRegex().Replace(boldItalicsReplaced, "");
        return new Markup(spansRemoved);
    }
}