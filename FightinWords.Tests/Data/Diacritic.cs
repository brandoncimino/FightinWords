using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using static FightinWords.SingleCharData;

namespace FightinWords.Data;

/// <summary>
/// Represents a <a href="https://en.wikipedia.org/wiki/Diacritic">diacritic</a>, aka "accent", that is used to modify other characters.
/// </summary>
/// <param name="Standalone">The visually isolated representation of the mark, such as <c>~</c></param>
/// <param name="Combining">The version of the mark that isn't displayed on its own, but instead combined with whatever came before it</param>
/// <param name="Modifier"><i>(optional)</i> The corresponding <a href="https://en.wikipedia.org/wiki/Modifier_letter">"modifier letter"</a>.
/// <br/>
/// Most definitions - and the name - of these characters, such as the one at <see cref="UnicodeCategory.ModifierLetter"/> are misleading:
/// they don't <i>modify</i> letters, they <b><i>are</i> letters</b>.
/// <br/>
/// This is used to represent letters like <a href="https://en.wikipedia.org/wiki/%CA%BBOkina">ʻOkina</a>, <c>ʻ</c>, which look like symbols but are actually distinct letters.
/// For example, the proper spelling of <a href="https://en.wikipedia.org/wiki/Hawaii">Hawaiʻi</a> has <b><i>7 letters</i></b>, not 6 as you might expect.
/// <br/>
/// In some ways, this makes them the inverse of <a href="https://en.wikipedia.org/wiki/Letterlike_Symbols">Letterlike Symbols</a>.
/// <br/>
/// The fact that modifier letters are sometimes used to transcribe letters with diacritics, such as in <a href="https://en.wikipedia.org/wiki/O%CA%BB">Oʻ</a>,
/// this is <b>objectively incorrect</b>: you could argue that <a href="https://en.wikipedia.org/wiki/O%CA%BB">Oʻ</a> is
/// <ul>
/// <li>1 letter</li>
/// <li>1 letter + 1 combining diacritic</li>
/// </ul>
/// But you <i>definitely</i> can't argue that it's <i>two letters</i>.
/// </param>
/// <remarks>
/// <ul>
/// <li>Unlike translations between <a href="https://en.wikipedia.org/wiki/Playing_cards_in_Unicode">playing cards in Unicode</a>, there is no consistent way to go between <see cref="Standalone"/> and <see cref="Combining"/> versions of a diacritic.</li>
/// <li>Some diacritics modify <b>multiple characters at once</b>, such as <see cref="Undertie"/>.</li>
/// </ul>
/// This class also contains <c>static</c> fields for diacritics that are:
/// <ul>
/// <li>Common, like <see cref="Tilde"/></li>
/// <li>Gnarly, like <see cref="Undertie"/></li>
/// <li>Funnily named, like <see cref="Ogonek"/></li>
/// </ul>
/// </remarks>
public readonly record struct Diacritic(char Standalone, char Combining, char? Modifier = default)
{
    public static readonly Diacritic Grave      = new(Grave_Standalone, Grave_Combining, Grave_Modifier);
    public static readonly Diacritic Acute      = new(Acute_Standalone, Acute_Combining, Acute_Modifier);
    public static readonly Diacritic Circumflex = new(Circumflex_Standalone, Circumflex_Combining, Circumflex_Modifier);
    public static readonly Diacritic Tilde      = new(Tilde_Standalone, Tilde_Combining);
    public static readonly Diacritic Macron     = new(Macron_Standalone, Macron_Combining);
    public static readonly Diacritic Trema      = new(Trema_Standalone, Trema_Combining);
    public static readonly Diacritic Tie        = new(Tie_Standalone, Tie_Combining);
    public static readonly Diacritic Undertie   = new(Undertie_Standalone, Undertie_Combining);
    public static readonly Diacritic Ogonek     = new(Ogonek_Standalone, Ogonek_Combining);
    public static readonly Diacritic Solidus    = new(Solidus_Standalone, Solidus_Combining);

    public static ImmutableHashSet<Diacritic> All => typeof(Diacritic).GetStaticData<Diacritic>().Values.ToImmutableHashSet();
}