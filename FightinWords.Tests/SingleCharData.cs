using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Unicode;
using JetBrains.Annotations;

namespace FightinWords;

public readonly record struct CharData(char Value)
{
    public string Name => UnicodeInfo.GetName(Value);

    public override string ToString()
    {
        return $"{Name} `{Value}`";
    }

    public static implicit operator CharData(char value) => new(value);
    public static implicit operator char(CharData value) => value.Value;
}

/// <summary>
/// Contains a bunch of <see cref="char"/>s that are useful for testing.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class SingleCharData
{
    public const char ZeroWidthJoiner      = '\u200D';
    public const char ReplacementCharacter = '\uFFFD';
    public const char MonoVariant          = '\uFE0E';
    public const char EmojiVariant         = '\uFE0F';

    public const char Space = ' ';
    public const char Tab   = '\t';

    public const char Okina     = 'ʻ';
    public const char Chillu_K  = 'ൿ';
    public const char Chillu_LL = 'ൾ';

    /// Classified as a "Letterlike Symbol".
    public const char DegreeCelsius = '℃';

    public const char LowerA  = 'a';
    public const char UpperAE = 'Æ';

    public const char LowerEnye = 'ñ';
    public const char Beta      = 'β';

    public const char Lambda           = 'λ';
    public const char PileOfDiacritics = 'ᾄ';

    public const char Period   = '.';
    public const char ThaiBaht = '฿';
    public const char Peseta   = '₧';

    public const char ExclamationPoint      = '!';
    public const char ExclamationPointEmoji = '❗';

    public const char One            = '1';
    public const char SuperZero      = '⁰';
    public const char OneOverNothing = '⅟';
    public const char OneOverTen     = '⅒';
    public const char RomanViii      = 'ⅷ';

    public const char HighSurrogate = '\ud83d' /* "High" (first; the order is backwards) surrogate of 📎 */;
    public const char LowSurrogate  = '\udcce' /* "Low" (second; the order is backwards) surrogate of 📎 */;

    #region Diacritics

    public const char Grave_Standalone = '`';
    public const char Grave_Combining  = '\u0300';
    public const char Grave_Modifier   = '\u02CB';

    public const char Acute_Standalone = '\u00B4';
    public const char Acute_Combining  = '\u0301';
    public const char Acute_Modifier   = '\u02CA';

    public const char Circumflex_Standalone = '^';
    public const char Circumflex_Combining  = '\u0302';
    public const char Circumflex_Modifier   = '\u02C6';

    public const char Tilde_Standalone    = '~';
    public const char Tilde_Combining     = '\u0303';
    public const char Trema_Standalone    = '\u00a8';
    public const char Tie_Standalone      = '\u2040';
    public const char Undertie_Standalone = '\u203F';
    public const char Ogonek_Standalone   = '\u02DB';
    public const char Solidus_Standalone  = '/';

    public const char Macron_Standalone  = '\u00af';
    public const char Macron_Combining   = '\u0304';
    public const char Trema_Combining    = '\u0308';
    public const char Tie_Combining      = '\u0361';
    public const char Undertie_Combining = '\u035C';
    public const char Ogonek_Combining   = '\u0328';
    public const char Solidus_Combining  = '\u0338';

    public static readonly ImmutableArray<char> Diacritics_Standalone =
    [
        Grave_Standalone,
        Acute_Standalone,
        Circumflex_Standalone,
        Tilde_Standalone,
        Macron_Standalone,
        Trema_Standalone,
        Tie_Standalone,
        Undertie_Standalone,
        Ogonek_Standalone,
        Solidus_Standalone,
    ];

    public static readonly ImmutableArray<char> Diacritics_Combining =
    [
        Grave_Combining,
        Acute_Combining,
        Circumflex_Combining,
        Tilde_Combining,
        Macron_Combining,
        Trema_Combining,
        Tie_Combining,
        Undertie_Combining,
        Ogonek_Combining,
        Solidus_Combining,
    ];

    #endregion


    private const int AsciiMax = 0x007F;

    public static readonly ImmutableList<char> AsciiChars = Enumerable.Range(0, AsciiMax + 1)
                                                                      .Select(it => (char)it)
                                                                      .ToImmutableList();


    public static readonly ImmutableHashSet<CharData> AllChars = typeof(SingleCharData).GetStaticData<char>()
        .Values
        .Select(it => new CharData(it))
        .ToImmutableHashSet();

    public static readonly ImmutableHashSet<CharData> Letters = [..AllChars.Where(it => char.IsLetter(it.Value))];
    public static readonly ImmutableHashSet<CharData> Numbers = [..AllChars.Where(it => char.IsNumber(it.Value))];
    public static readonly ImmutableHashSet<CharData> Symbols = [..AllChars.Where(it => char.IsSymbol(it.Value))];

    public static readonly ImmutableHashSet<CharData> DefinitelyNotGraphemes =
    [
        ..AllChars.Where(it => char.IsWhiteSpace(it.Value) || char.IsControl(it.Value) || char.IsSurrogate(it.Value)),
        ZeroWidthJoiner
    ];
}