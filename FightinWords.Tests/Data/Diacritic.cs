using System.Globalization;
using System.Text;

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
    public static readonly Diacritic Grave      = new('`', '\u0300', '\u02CB');
    public static readonly Diacritic Acute      = new('\u00B4', '\u0301', '\u02CA');
    public static readonly Diacritic Circumflex = new('^', '\u0302', '\u02C6');
    public static readonly Diacritic Tilde      = new('~', '\u0303');
    public static readonly Diacritic Macron     = new('\u00af', '\u0304');
    public static readonly Diacritic Trema      = new('\u00a8', '\u0308');
    public static readonly Diacritic Tie        = new('\u2040', '\u0361');
    public static readonly Diacritic Undertie   = new('\u203F', '\u035C');
    public static readonly Diacritic Ogonek     = new('\u02DB', '\u0328');
    public static readonly Diacritic Solidus    = new('/', '\u0338');
}