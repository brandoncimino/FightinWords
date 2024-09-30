using System.Collections.Immutable;
using System.Text;
using System.Unicode;
using FightinWords.Data;

namespace FightinWords;

/// <summary>
/// <see cref="DatapointAttribute"/>s are an interesting idea, and something that I've considered for a while;
/// but they have several issues:
/// <ul>
/// <li>They clog up your results with massive piles of "inconclusive" tests, which is a problem when "inconclusive" is usually an indication that a test didn't execute properly</li>
/// <li><see cref="DatapointsAttribute"/>, the plural version, just doesn't seem to work 🤷‍♀️</li>
/// </ul>
/// </summary>
public static class TestData
{
    public readonly record struct StringData(string Content)
    {
        public int Length => Content.Length;
        public override string ToString()
        {
            var me = this;
            return Content
                          .Select(it => $"[{UnicodeInfo.GetName(it)} `{it}`]")
                          .JoinString("", $"({me.Length})");
        }

        public static implicit operator StringData(string   s)    => new(s);
        public static implicit operator StringData(char     c)    => new(c.ToString());
        public static implicit operator StringData(Rune     r)    => new(r.ToString());
        public static implicit operator StringData(CharData c)    => c.Value;
    }
    
    public readonly record struct SurrogatePair(char High, char Low)
    {
        public bool IsValid => char.IsSurrogatePair(High, Low);

        public SurrogatePair(string s) : this(RequirePair(s))
        {

        }

        public SurrogatePair((char high, char low) pair) : this(pair.high, pair.low){}

        private static (char high, char low) RequirePair(string s)
        {
            if (s is not [var high, var low])
            {
                throw new ArgumentException($"`{s}` wasn't exactly 2 characters long");
            }

            return (high, low);
        }

        public static implicit operator SurrogatePair(string s)    => new(s);
        public static implicit operator StringData(SurrogatePair pair) => $"{pair.High}{pair.Low}";
        public static implicit operator SurrogatePair(Rune   r)    => new(r.ToString());
        public static implicit operator Rune(SurrogatePair   pair) => new(pair.High, pair.Low);
    }

    public static readonly ImmutableArray<SurrogatePair> SurrogatePair_Letters =
    [
        new(@"𝝫") /* MATHEMATICAL SANS-SERIF BOLD CAPITAL PHI */,
        new(@"𝐓") /* MATHEMATICAL BOLD CAPITAL T */,
    ];

    public static readonly ImmutableArray<SurrogatePair> SurrogatePair_Numbers =
    [
        @"𝟞",
    ];

    public static readonly ImmutableArray<SurrogatePair> SurrogatePair_Symbols =
    [
        @"📎",
        @"🎂",
    ];

    public const  char   HighSurrogate_Paperclip   = '\ud83d';
    public const  char   LowSurrogate_Paperclip    = '\udcce';
    public const  string RainbowFlag               = @"🏳️‍🌈";
    public const  string BlackGirlRunning          = @"🏃🏿‍♀️";
    public const  string TwoDadsWithSon            = @"👨‍👨‍👦";
    /// <summary>
    /// Malayalam Chillu-LL in Unicode &lt; 5.1 (where it became the single `ൾ`) - Particularly notable because it *ENDS* in a ZWJ, which definitely causes lots of problems
    /// </summary>
    public const string MayalamChillu_LL_Obsolete = "ള\u0d4d\u200d";

    public const string JollyRoger_EmojiSelected = "\ud83c\udff4\u200d\u2620\ufe0f";

    private static ImmutableArray<SurrogatePair> CreateInvalidPairs(string validSurrogatePair)
    {
        if (validSurrogatePair is not [var high, var low])
        {
            throw new ArgumentException("Not a surrogate pair!", nameof(validSurrogatePair));
        }

        var badPairs = CartesianProduct.Of(
                                           [high, low, 'a'],
                                           [high, low, 'a']
                                       )
                                       .Where(it => it != (high, low))
                                       .Select(it => new SurrogatePair(it.a, it.b))
                                       .ToImmutableArray();

        return badPairs;
    }

    public static readonly ImmutableArray<string> MultiCodeSequences_Valid =
    [
        MayalamChillu_LL_Obsolete,
        RainbowFlag,
        BlackGirlRunning,
        TwoDadsWithSon,
        JollyRoger_EmojiSelected,
    ];

    public static ImmutableArray<StringData> GetValidGraphemeClusters()
    {
        return
        [
            ..MultiCodeSequences_Valid,
            ..SurrogatePair_Letters,
            ..SurrogatePair_Numbers,
            ..SurrogatePair_Symbols,
            ..SingleCharData.Letters,
            ..SingleCharData.Numbers
        ];
    }
    
    public static ImmutableHashSet<StringData> GetInvalidGraphemeClusters()
    {
        return
        [
            ..CreateInvalidPairs("📎"),
            $"{HighSurrogate_Paperclip}",
            $"{LowSurrogate_Paperclip}",
            $"{SingleCharData.EmojiVariant}{SingleCharData.MonoVariant}{SingleCharData.EmojiVariant}" /* "orphaned" variant selectors */,
            "",
            " ",
            "  ",
            "a ",
            " a",
            $" {Diacritic.Acute.Combining}",
            $"{Diacritic.Ogonek.Combining}",
            $"{Diacritic.Ogonek.Combining}{Diacritic.Ogonek.Combining}",
            $"{Diacritic.Trema.Combining}",
            $"{Diacritic.Trema.Combining}a",
            $"/{Diacritic.Trema.Combining}{Diacritic.Trema.Combining}",
            $" {Diacritic.Trema.Combining}a",
            ..SingleCharData.DefinitelyNotGraphemes,
        ];
    }
}