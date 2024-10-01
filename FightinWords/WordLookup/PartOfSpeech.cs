namespace FightinWords.WordLookup;

public enum PartOfSpeech
{
    Noun,
    Verb,
    Adjective,
    Conjunction,
    /// <summary>
    /// Some weird thing that Wiktionary returned for the Gothic usage of "and":
    /// <code><![CDATA[
    /// {
    ///   "partOfSpeech": "Romanization",
    ///   "language": "Gothic",
    ///   "definitions": [
    ///     {
    ///       "definition": "<span class=\"form-of-definition use-with-mention\" about=\"#mwt256\" typeof=\"mw:Transclusion\">Romanization of <span class=\"form-of-definition-link\"><i class=\"Goth mention\" lang=\"got\"><a rel=\"mw:WikiLink\" href=\"/wiki/𐌰𐌽𐌳#Gothic\" title=\"𐌰𐌽𐌳\">𐌰𐌽𐌳</a></i></span></span>"
    ///     }
    ///   ]
    /// }
    /// ]]></code>
    /// </summary>
    Romanization,
    /// <summary>
    /// Represents multiple words (even if its only one word itself), such as <a href="https://en.wiktionary.org/wiki/YOLO#English">YOLO</a>.
    /// </summary>
    Phrase,
    /// <summary>
    /// Stuff like the <a href="https://en.wiktionary.org/wiki/sag#Translingual">"sag" language code</a>.
    /// </summary>
    Symbol,
}