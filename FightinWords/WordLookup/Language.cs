namespace FightinWords.WordLookup;

public enum Language
{
    English,
    German,
    Dutch,
    Spanish,
    Italian,
    Gothic,
    Azerbaijan,
    Danish,
    Estonian,
    Scots,
    Swedish,
    Turkish,
    Afrikaans,
    /// <summary>
    /// I believe this is used by Wiktionary to mark super, <i>super</i> exotic languages like <a href="https://en.wikipedia.org/wiki/Fingallian">Fingallian</a>, <a href="https://en.wikipedia.org/wiki/Livonian_language">Livonian</a>, and <a href="https://en.wikipedia.org/wiki/Yola_dialect">Yola</a>.
    /// <example>
    /// Response from <c>https://en.wiktionary.org/api/rest_v1/page/definition/and</c> entry <c>"other"</c>:
    /// <code><![CDATA[
    /// [
    ///   {
    ///     "partOfSpeech": "Conjunction",
    ///     "language": "Fingallian",
    ///     "definitions": [
    ///       {
    ///         "definition": "<span class=\"Latn\" lang=\"en\" about=\"#mwt254\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink\" href=\"/wiki/and#English\" class=\"mw-selflink-fragment\">and</a></span>"
    ///       }
    ///     ]
    ///   },
    ///   {
    ///     "partOfSpeech": "Verb",
    ///     "language": "Livonian",
    ///     "definitions": [
    ///       {
    ///         "definition": "<span class=\"usage-label-sense\" about=\"#mwt263\" typeof=\"mw:Transclusion\"></span> to <a rel=\"mw:WikiLink\" href=\"/wiki/give\" title=\"give\">give</a>"
    ///       }
    ///     ]
    ///   },
    ///   {
    ///     "partOfSpeech": "Conjunction",
    ///     "language": "Middle English",
    ///     "definitions": [
    ///       {
    ///         "definition": "<span class=\"Latn\" lang=\"en\" about=\"#mwt276\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink\" href=\"/wiki/and#English\" class=\"mw-selflink-fragment\">and</a></span>, <span class=\"Latn\" lang=\"en\" about=\"#mwt277\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink\" href=\"/wiki/and#English\" class=\"mw-selflink-fragment\">and</a></span> <a rel=\"mw:WikiLink\" href=\"/wiki/then\" title=\"then\">then</a> <span class=\"gloss-brac\" about=\"#mwt278\" typeof=\"mw:Transclusion\">(</span><span class=\"gloss-content\" about=\"#mwt278\"><span class=\"Latn\" lang=\"en\">connects two elements of a sentence</span></span><span class=\"gloss-brac\" about=\"#mwt278\">)</span>"
    ///       },
    ///       {
    ///         "definition": "<a rel=\"mw:WikiLink\" href=\"/wiki/however\" title=\"however\">however</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/yet\" title=\"yet\">yet</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/but\" title=\"but\">but</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/though\" title=\"though\">though</a>. <a rel=\"mw:WikiLink\" href=\"/wiki/while\" title=\"while\">while</a>"
    ///       },
    ///       {
    ///         "definition": "<a rel=\"mw:WikiLink\" href=\"/wiki/if\" title=\"if\">if</a>, supposing that, <a rel=\"mw:WikiLink\" href=\"/wiki/whether\" title=\"whether\">whether</a>."
    ///       },
    ///       {
    ///         "definition": "<span class=\"usage-label-sense\" about=\"#mwt284\" typeof=\"mw:Transclusion\"></span> <a rel=\"mw:WikiLink\" href=\"/wiki/as\" title=\"as\">As</a> <a rel=\"mw:WikiLink\" href=\"/wiki/though\" title=\"though\">though</a>, like, in a manner suggesting."
    ///       }
    ///     ]
    ///   },
    ///   {
    ///     "partOfSpeech": "Noun",
    ///     "language": "Norwegian Bokmål",
    ///     "definitions": [
    ///       {
    ///         "definition": "a <span class=\"Latn\" lang=\"en\" about=\"#mwt304\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink\" href=\"/wiki/duck#English\" title=\"duck\">duck</a></span>"
    ///       },
    ///       {
    ///         "definition": "<a rel=\"mw:WikiLink\" href=\"/wiki/canard\" title=\"canard\">canard</a> (false or misleading report or story)"
    ///       }
    ///     ]
    ///   },
    ///   {
    ///     "partOfSpeech": "Noun",
    ///     "language": "Norwegian Nynorsk",
    ///     "definitions": [
    ///       {
    ///         "definition": "a <span class=\"Latn\" lang=\"en\" about=\"#mwt318\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink\" href=\"/wiki/duck#English\" title=\"duck\">duck</a></span>"
    ///       }
    ///     ]
    ///   },
    ///   {
    ///     "partOfSpeech": "Noun",
    ///     "language": "Norwegian Nynorsk",
    ///     "definitions": [
    ///       {
    ///         "definition": ""
    ///       },
    ///       {
    ///         "definition": "<a rel=\"mw:WikiLink\" href=\"/wiki/breath\" title=\"breath\">breath</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/spirit\" title=\"spirit\">spirit</a>"
    ///       }
    ///     ]
    ///   },
    ///   {
    ///     "partOfSpeech": "Verb",
    ///     "language": "Norwegian Nynorsk",
    ///     "definitions": [
    ///       {
    ///         "definition": "<span class=\"form-of-definition use-with-mention\" about=\"#mwt330\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink\" href=\"/wiki/Appendix:Glossary#imperative_mood\" title=\"Appendix:Glossary\">imperative</a> of <span class=\"form-of-definition-link\"><i class=\"Latn mention\" lang=\"nn\"><a rel=\"mw:WikiLink\" href=\"/wiki/ande#Norwegian_Nynorsk\" title=\"ande\">ande</a></i></span></span>"
    ///       }
    ///     ]
    ///   },
    ///   {
    ///     "partOfSpeech": "Conjunction",
    ///     "language": "Old English",
    ///     "definitions": [
    ///       {
    ///         "definition": "<span class=\"Latn\" lang=\"en\" about=\"#mwt343\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink\" href=\"/wiki/and#English\" class=\"mw-selflink-fragment\">and</a></span>"
    ///       }
    ///     ]
    ///   },
    ///   {
    ///     "partOfSpeech": "Adverb",
    ///     "language": "Old English",
    ///     "definitions": [
    ///       {
    ///         "definition": "<a rel=\"mw:WikiLink\" href=\"/wiki/even\" title=\"even\">even</a>; <a rel=\"mw:WikiLink\" href=\"/wiki/also\" title=\"also\">also</a>"
    ///       }
    ///     ]
    ///   },
    ///   {
    ///     "partOfSpeech": "Conjunction",
    ///     "language": "Old Frisian",
    ///     "definitions": [
    ///       {
    ///         "definition": "<a rel=\"mw:WikiLink\" href=\"/wiki/and#English\" class=\"mw-selflink-fragment\">and</a>"
    ///       }
    ///     ]
    ///   },
    ///   {
    ///     "partOfSpeech": "Pronoun",
    ///     "language": "Old Irish",
    ///     "definitions": [
    ///       {
    ///         "definition": "<span class=\"form-of-definition use-with-mention\" about=\"#mwt376\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink\" href=\"/wiki/Appendix:Glossary#third_person\" title=\"Appendix:Glossary\">third-person</a> <a rel=\"mw:WikiLink\" href=\"/wiki/Appendix:Glossary#singular_number\" title=\"Appendix:Glossary\">singular</a> <span class=\"inflection-of-conjoined\"><a rel=\"mw:WikiLink\" href=\"/wiki/Appendix:Glossary#gender\" title=\"Appendix:Glossary\">masculine</a><span class=\"inflection-of-sep\">/</span><a rel=\"mw:WikiLink\" href=\"/wiki/Appendix:Glossary#gender\" title=\"Appendix:Glossary\">neuter</a></span> <a rel=\"mw:WikiLink\" href=\"/wiki/Appendix:Glossary#dative_case\" title=\"Appendix:Glossary\">dative</a> of <span class=\"form-of-definition-link\"><i class=\"Latn mention\" lang=\"sga\"><a rel=\"mw:WikiLink\" href=\"/wiki/hi#Old_Irish\" title=\"hi\">hi</a></i></span></span>: <a rel=\"mw:WikiLink\" href=\"/wiki/in\" title=\"in\">in</a> <a rel=\"mw:WikiLink\" href=\"/wiki/him\" title=\"him\">him</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/in\" title=\"in\">in</a> <a rel=\"mw:WikiLink\" href=\"/wiki/it\" title=\"it\">it</a>"
    ///       }
    ///     ]
    ///   },
    ///   {
    ///     "partOfSpeech": "Adverb",
    ///     "language": "Old Irish",
    ///     "definitions": [
    ///       {
    ///         "definition": "<a rel=\"mw:WikiLink\" href=\"/wiki/there\" title=\"there\">there</a>"
    ///       },
    ///       {
    ///         "definition": "<a rel=\"mw:WikiLink\" href=\"/wiki/then\" title=\"then\">then</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/in_that_case\" title=\"in that case\">in that case</a>"
    ///       }
    ///     ]
    ///   },
    ///   {
    ///     "partOfSpeech": "Conjunction",
    ///     "language": "Yola",
    ///     "definitions": [
    ///       {
    ///         "definition": "<span class=\"form-of-definition use-with-mention\" about=\"#mwt423\" typeof=\"mw:Transclusion\">Alternative form of <span class=\"form-of-definition-link\"><i class=\"Latn mention\" lang=\"yol\"><a rel=\"mw:WikiLink\" href=\"/wiki/an#Yola\" title=\"an\">an</a></i> <span class=\"mention-gloss-paren annotation-paren\">(</span><span class=\"mention-gloss-double-quote\">“</span><span class=\"mention-gloss\">and</span><span class=\"mention-gloss-double-quote\">”</span><span class=\"mention-gloss-paren annotation-paren\">)</span></span></span>"
    ///       }
    ///     ]
    ///   },
    ///   {
    ///     "partOfSpeech": "Noun",
    ///     "language": "Zealandic",
    ///     "definitions": [
    ///       {
    ///         "definition": "<link rel=\"mw:PageProp/Category\" href=\"./Category:zea:Body_parts#AND\" about=\"#mwt432\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink\" href=\"/wiki/and#English\" class=\"mw-selflink-fragment\">hand</a>"
    ///       }
    ///     ]
    ///   }
    /// ]
    /// ]]></code>
    /// </example>
    /// </summary>
    Other,
}

public static class IsoLanguageExtensions
{
    public static string LanguageCode(this Language language)
    {
        return language switch
               {
                   Language.English => "en",
                   Language.German  => "de",
                   Language.Dutch   => "nl",
                   Language.Spanish => "es",
                   Language.Italian => "it",
                   Language.Gothic => "got",
                   Language.Azerbaijan => "az",
                   Language.Danish => "da",
                   Language.Estonian => "et",
                   Language.Scots => "sco",
                   Language.Swedish => "sv",
                   Language.Turkish => "tr",
                   Language.Afrikaans => "ak",
                   Language.Other => "other",
                   _                => throw new ArgumentOutOfRangeException(nameof(language), language, null)
               };
    }
}