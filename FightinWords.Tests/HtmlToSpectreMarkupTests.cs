using FightinWords.Console;
using FightinWords.Data;
using Spectre.Console;

namespace FightinWords;

public class HtmlToSpectreMarkupTests
{
    public const string OneHtml = """
                                  <span class="usage-label-sense" about="#mwt12" typeof="mw:Transclusion"></span><span about="#mwt12"> </span><span class="use-with-mention" about="#mwt12"><span class="Latn" lang="en"><a rel="mw:WikiLink/Interwiki" href="https://en.wikipedia.org/wiki/NATO" title="w:NATO" class="extiw">NATO</a> <span typeof="mw:Entity">&amp;</span> <a rel="mw:WikiLink/Interwiki" href="https://en.wikipedia.org/wiki/International%20Civil%20Aviation%20Organization" title="w:International Civil Aviation Organization" class="extiw">ICAO</a> <a rel="mw:WikiLink/Interwiki" href="https://en.wikipedia.org/wiki/NATO%20phonetic%20alphabet" title="w:NATO phonetic alphabet" class="extiw">radiotelephony clear code</a> (spelling-alphabet name) for the digit <i class="None mention" lang="mul"><a rel="mw:WikiLink" href="/wiki/1#Translingual" title="1">1</a></i>.</span></span><link rel="mw:PageProp/Category" href="./Category:ICAO_spelling_alphabet" about="#mwt12">
                                  """;

    public const string OnePlain = "NATO & ICAO radiotelephony clear code (spelling-alphabet name) for the digit 1.";

    public const string SonHtml = """
                                  <span class="usage-label-sense" about="#mwt6" typeof="mw:Transclusion"></span><span about="#mwt6"> </span><i about="#mwt6"><a rel="mw:WikiLink/Interwiki" href="https://en.wikipedia.org/wiki/ISO%20639-2" title="w:ISO 639-2" class="extiw">ISO 639-2</a><link rel="mw:PageProp/Category" href="./Category:ISO_639-2"> &amp; <a rel="mw:WikiLink/Interwiki" href="https://en.wikipedia.org/wiki/ISO%20639-5" title="w:ISO 639-5" class="extiw">ISO 639-5</a><link rel="mw:PageProp/Category" href="./Category:ISO_639-5"><link rel="mw:PageProp/Category" href="./Category:Theknightwho's_maintenance_category"> <a rel="mw:WikiLink" href="/wiki/language_code" title="language code">language code</a> for </i><a rel="mw:WikiLink/Interwiki" href="https://en.wikipedia.org/wiki/ISO%20639:son" title="w:ISO 639:son" about="#mwt6" class="extiw"><b>Songhay languages</b></a><i about="#mwt6">.</i>
                                  """;

    public const string SonPlain = "ISO 639-2 & ISO 639-5 language code for Songhay languages.";

    [TestCase(SonHtml, SonPlain)]
    [TestCase(OneHtml, OnePlain)]
    public void HtmlToMarkupTest(string html, string plain)
    {
        var markup = HtmlToSpectreMarkup.Convert(html);

        var cleaned = Markup.Remove(markup);
        Assert.That(cleaned, Is.EqualTo(plain));
    }

    [Test]
    [TestCase("""<i class="swag">yolo</i>""",                 "[i]yolo[/]")]
    [TestCase("""<i class="swag">yolo</i> <i>swaggins</i>""", "[i]yolo[/] [i]swaggins[/]")]
    [TestCase("""<i>before <i>inside</i> after</i>""",        "[i]before [i]inside[/] after[/]")]
    [TestCase("""<i>before <b>inside</b> after</i>""",        "[i]before [b]inside[/] after[/]")]
    public void ReplaceBoldItalics(string html, string expectedAfterReplace)
    {
        var replacedBoldItalics = HtmlToSpectreMarkup.ReplaceBoldItalics(html);
        System.Console.WriteLine(replacedBoldItalics);
        TestHelpers.AssertEquals(replacedBoldItalics, expectedAfterReplace);
    }

    public static IEnumerable<CharacterReference> GetCharacterReferences()
    {
        return TestHelpers.GetStaticData<CharacterReference>(typeof(CharacterReference))
                          .Values;
    }

    [Test]
    public void GetStringForCharacterReferenceTest(
        [ValueSource(nameof(GetCharacterReferences))]
        CharacterReference characterReference
    )
    {
        var actualResult = HtmlToSpectreMarkup.GetStringForCharacterReference(characterReference.Html.AsSpan()[1..^1]);
        TestHelpers.AssertEquals(actualResult, characterReference.DisplayedCharacter);
    }

    [Test]
    [TestCase(
        """
        <a rel="mw:WikiLink/Interwiki" href="https://en.wikipedia.org/wiki/NATO" title="w:NATO" class="extiw">NATO</a>
        """,
        "[link=https://en.wikipedia.org/wiki/NATO]NATO[/]"
    )]
    public void ReplaceHyperLinksTest(string html, string expectedAfterReplace)
    {
        var hyperlinksReplaced = HtmlToSpectreMarkup.ReplaceHyperlinks(html);
        TestHelpers.AssertEquals(hyperlinksReplaced, expectedAfterReplace);
    }
}