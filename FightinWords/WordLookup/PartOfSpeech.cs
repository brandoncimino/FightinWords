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
}