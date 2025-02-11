using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace FightinWords.Console;

public static partial class HtmlToSpectreMarkup
{
    public static string Convert(string html)
    {
        html = Markup.Escape(html);
        html = RemoveUselessTags(html);
        html = ReplaceBoldItalics(html);
        html = ReplaceHyperlinks(html);
        html = ReplaceHtmlCharacterReferences(html);
        html = html.Trim();
        html = CollapseSpaces(html);
        return html;
    }

    #region Bold & Italics

    [GeneratedRegex("""<\s*([ib]).*?>""")]
    private static partial Regex BoldItalicRegex_StartTag();

    [GeneratedRegex("""</\s*[ib]\s*>""")]
    private static partial Regex BoldItalicRegex_EndTag();

    /// <summary>
    /// Replaces <c>&lt;i></c> and <c>&lt;b></c> tags with their corresponding <see cref="Spectre.Console.Decoration"/>'s <see cref="Spectre.Console.Markup"/>.
    /// </summary>
    /// <param name="html">a string that <i>might</i> contain HTML text decorations.</param>
    /// <remarks>
    /// Previously, this used a single regex, which looked for matching start and end tags. However, that didn't deal with nested tags well.
    /// Splitting it into two regexes is more likely to replace everything, however, it will also replace "unmatched" start and end tags.
    /// </remarks>
    [Pure]
    internal static string ReplaceBoldItalics(string html)
    {
        html = BoldItalicRegex_StartTag().Replace(html, "[$1]");
        html = BoldItalicRegex_EndTag().Replace(html, "[/]");
        return html;
    }

    #endregion

    /// <summary>
    /// Finds both start and end tags for HTML elements that we don't care about, like <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/span">&lt;span></a>
    /// and <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/link">&lt;link></a>.
    ///
    /// Note that this preserves their content.
    /// </summary>
    [GeneratedRegex("""
                    <\s*/?\s*(span|link).*?>
                    """
    )]
    private static partial Regex UselessTagRegex();

    [Pure]
    private static string RemoveUselessTags(string html) => UselessTagRegex().Replace(html, "");

    /// <summary>
    /// Matches <![CDATA[<a href="...">]]> and <![CDATA[<link href="...">]]> elements' start and end tags.
    /// <br/>
    /// Captures their <c>href</c> attribute into the group <c>"href"</c> and their contents <i>(i.e. in-between the start and end tags)</i> into the group <c>"text"</c>.
    /// </summary>
    [GeneratedRegex("""<\s*a.*?href\s*=\s*"(?<href>.*?)".*?>(?<text>.*?)</a>""")]
    private static partial Regex HyperlinkRegex();

    [Pure]
    internal static string ReplaceHyperlinks(string html) => HyperlinkRegex().Replace(html, "[link=$1]$2[/]");


    [GeneratedRegex("&(.*?);")]
    private static partial Regex CharacterReferenceRegex();

    /// <summary>
    /// Replaces <a href="https://developer.mozilla.org/en-US/docs/Glossary/Character_reference">HTML Character References</a> with their corresponding symbols.
    /// <br/>
    /// <br/>
    /// Supports:
    /// <ul>
    /// <li><i>(Some)</i> Named references, like <c><![CDATA[&amp;]]></c> -> <c>&amp;</c></li>
    /// <li>Decimal references, like <c><![CDATA[&#97;]]></c> -> <c>a</c></li>
    /// <li>Hexadecimal references, like <c><![CDATA[&#x0061]]></c> -> <c>a</c></li>
    /// </ul>
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    [Pure]
    private static string ReplaceHtmlCharacterReferences(string html)
    {
        return CharacterReferenceRegex()
            .Replace(
                html,
                it => GetStringForCharacterReference(it.Groups[1].ValueSpan)
            );
    }

    /// <summary>
    /// Retrieves the <see cref="string"/> representation of the <i>meaningful portion</i> of an <a href="https://developer.mozilla.org/en-US/docs/Glossary/Character_reference">HTML Character Reference</a>.
    /// <br/>
    /// <br/>
    /// </summary>
    /// <remarks>
    /// The only <b>named references</b> that this works with are the "common ones" listed on <a href="https://developer.mozilla.org/en-US/docs/Glossary/Character_reference">Mozilla's reference page</a>:
    /// <code><![CDATA[
    /// Character	Named reference	Unicode code-point
    /// 	&amp;amp;	U+00026
    /// <	&amp;lt;	U+0003C
    /// >	&amp;gt;	U+0003E
    /// "	&amp;quot;	U+00022
    /// '	&amp;apos;	U+00027
    ///  	&amp;nbsp;	U+000A0
    /// –	&amp;ndash;	U+02013
    /// —	&amp;mdash;	U+02014
    /// ©	&amp;copy;	U+000A9
    /// ®	&amp;reg;	U+000AE
    /// ™	&amp;trade;	U+02122
    /// ≈	&amp;asymp;	U+02248
    /// ≠	&amp;ne;	U+02260
    /// £	&amp;pound;	U+000A3
    /// €	&amp;euro;	U+020AC
    /// °	&amp;deg;	U+000B0
    /// ]]></code>
    /// </remarks>
    /// <param name="characterReference"></param>
    /// <returns></returns>
    public static string GetStringForCharacterReference(ReadOnlySpan<char> characterReference)
    {
        return characterReference switch
        {
            "amp"   => "&",
            "lt"    => "<",
            "gt"    => ">",
            "quot"  => "\"",
            "apos"  => "'",
            "nbsp"  => " ",
            "ndash" => "–",
            "mdash" => "—",
            "copy"  => "©",
            "reg"   => "®",
            "trade" => "™",
            "asymp" => "≈",
            "ne"    => "≠",
            "pound" => "£",
            "euro"  => "€",
            "deg"   => "°",
            ['#', 'x', .. var hex] => new Rune(int.Parse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                .ToString(),
            ['#', .. var dec] => new Rune(int.Parse(dec, NumberStyles.Integer, CultureInfo.InvariantCulture))
                .ToString(),
            _ => throw new ArgumentException($"`{characterReference}` isn't a known HTML character reference")
        };
    }

    [GeneratedRegex("""\ \ +""")]
    private static partial Regex MultipleSpacesRegex();

    [Pure]
    private static string CollapseSpaces(string html) => MultipleSpacesRegex().Replace(html, " ");
}