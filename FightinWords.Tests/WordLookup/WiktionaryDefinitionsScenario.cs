using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using FightinWords.WordLookup.Wiktionary;

namespace FightinWords.WordLookup;

public record WiktionaryDefinitionsScenario(
    string                        RequestedWord,
    Language                      RequestedLanguage,
    [StringSyntax("JSON")] string WiktionaryResponseJson,
    ImmutableList<WordDefinition> ExpectedDefinitions
)
{
    private static WiktionaryDefinitionsScenario Create(
        string                              word,
        [StringSyntax("JSON")]
        string                              responseJson,
        Language                            language,
        IEnumerable<(PartOfSpeech, string)> definitions
    )
    {
        var realDefinitions = definitions.Select(it => new WordDefinition(word, language, it.Item1, it.Item2))
                                         .ToImmutableList();
        return new WiktionaryDefinitionsScenario(word, language, responseJson, realDefinitions);
    }

    public static readonly WiktionaryDefinitionsScenario Kitten_English_Simplified = Create(
        "kitten",
        """
        {
          "en": [
            {
          "partOfSpeech": "Noun",
          "language": "English",
          "definitions": [
            {
          "definition": "Baby cat"
        },
            {
              "definition": "Baby non-cat"
            },
            {
              "definition": "Moth"
            },
            {
              "definition": "Girlfriend"
            }
          ]
        },
            {
              "partOfSpeech": "Verb",
              "language": "English",
              "definitions": [
                {
                  "definition": "Give birth"
                }
              ]
            }
          ],
          "nl": [
            {
              "partOfSpeech": "Noun",
              "language": "Dutch",
              "definitions": [
                {
                  "definition": "Dutch baby cat"
                }
              ]
            },
            {
              "partOfSpeech": "Verb",
              "language": "Dutch",
              "definitions": [
                {
                  "definition": "To apply sealant"
                }
              ]
            },
            {
              "partOfSpeech": "Noun",
              "language": "Dutch",
              "definitions": [
                {
                  "definition": "Plural of kit"
                }
              ]
            }
          ],
          "de": [
            {
              "partOfSpeech": "Verb",
              "language": "German",
              "definitions": [
                {
                  "definition": "To putty or cement"
                }
              ]
            }
          ]
        }
        """,
        Language.English,
        [
            (PartOfSpeech.Noun, "Baby cat"),
            (PartOfSpeech.Noun, "Baby non-cat"),
            (PartOfSpeech.Noun, "Moth"),
            (PartOfSpeech.Noun, "Girlfriend"),
            (PartOfSpeech.Verb, "Give birth")
        ]
    );

    public static readonly WiktionaryDefinitionsScenario Kitten_English = Create(
        WiktionaryModel.DefinitionsResponse.ExampleWord,
        WiktionaryModel.DefinitionsResponse.Example,
        Language.English,
        [
            (PartOfSpeech.Noun,
                "A <a rel=\"mw:WikiLink\" href=\"/wiki/young\" title=\"young\">young</a> <a rel=\"mw:WikiLink\" href=\"/wiki/cat\" title=\"cat\">cat</a>, especially before <a rel=\"mw:WikiLink\" href=\"/wiki/sexual\" title=\"sexual\">sexual</a> <a rel=\"mw:WikiLink\" href=\"/wiki/maturity\" title=\"maturity\">maturity</a> (reached at about seven months)."),
            (PartOfSpeech.Noun,
                "A young <a rel=\"mw:WikiLink\" href=\"/wiki/rabbit\" title=\"rabbit\">rabbit</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/rat\" title=\"rat\">rat</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/hedgehog\" title=\"hedgehog\">hedgehog</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/squirrel\" title=\"squirrel\">squirrel</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/fox\" title=\"fox\">fox</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/beaver\" title=\"beaver\">beaver</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/badger\" title=\"badger\">badger</a>, etc."),
            (PartOfSpeech.Noun,
                "A <a rel=\"mw:WikiLink\" href=\"/wiki/moth\" title=\"moth\">moth</a> of the genus <span class=\"biota\" about=\"#mwt35\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink/Interwiki\" href=\"https://species.wikimedia.org/wiki/Furcula\" title=\"wikispecies:Furcula\" class=\"extiw\"><i>Furcula</i></a><link rel=\"mw:PageProp/Category\" href=\"./Category:Entries_using_missing_taxonomic_name_(genus)#Furcula\"></span>."),
            (PartOfSpeech.Noun,
                "<span class=\"usage-label-sense\" about=\"#mwt37\" typeof=\"mw:Transclusion\"></span> <span class=\"use-with-mention\" about=\"#mwt38\" typeof=\"mw:Transclusion\"><span class=\"Latn\" lang=\"en\">A <a rel=\"mw:WikiLink\" href=\"/wiki/term_of_endearment#English\" title=\"term of endearment\">term of endearment</a>, especially for a woman.</span></span>"),
            (PartOfSpeech.Verb,
                "to give <a rel=\"mw:WikiLink\" href=\"/wiki/birth\" title=\"birth\">birth</a> to kittens")
        ]
    );

    public static readonly WiktionaryDefinitionsScenario Kitten_Dutch = Create(
        WiktionaryModel.DefinitionsResponse.ExampleWord,
        WiktionaryModel.DefinitionsResponse.Example,
        Language.Dutch,
        [
            (PartOfSpeech.Noun,
                "a young <a rel=\"mw:WikiLink\" href=\"/wiki/cat\" title=\"cat\">cat</a>; <span class=\"Latn\" lang=\"en\" about=\"#mwt313\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink\" href=\"/wiki/kitten#English\" class=\"mw-selflink-fragment\">kitten</a></span>"),
            (PartOfSpeech.Verb,
                "<span class=\"usage-label-sense\" about=\"#mwt320\" typeof=\"mw:Transclusion\"></span> to apply <a rel=\"mw:WikiLink\" href=\"/wiki/sealant\" title=\"sealant\">sealant</a> to"),
            (PartOfSpeech.Noun,
                "<span class=\"form-of-definition use-with-mention\" about=\"#mwt327\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink\" href=\"/wiki/Appendix:Glossary#plural_number\" title=\"Appendix:Glossary\">plural</a> of <span class=\"form-of-definition-link\"><i class=\"Latn mention\" lang=\"nl\"><a rel=\"mw:WikiLink\" href=\"/wiki/kit#Dutch\" title=\"kit\">kit</a></i></span></span>")
        ]
    );

    public static readonly WiktionaryDefinitionsScenario LanguageNotPresent = new(
        "xxx",
        Language.Scots,
        """
        {
          "en": [
            {
          "partOfSpeech": "Noun",
          "language": "English",
          "definitions": [
            {
          "definition": "A <a rel=\"mw:WikiLink\" href=\"/wiki/young\" title=\"young\">young</a> <a rel=\"mw:WikiLink\" href=\"/wiki/cat\" title=\"cat\">cat</a>, especially before <a rel=\"mw:WikiLink\" href=\"/wiki/sexual\" title=\"sexual\">sexual</a> <a rel=\"mw:WikiLink\" href=\"/wiki/maturity\" title=\"maturity\">maturity</a> (reached at about seven months)."
        },
            {
              "definition": "A young <a rel=\"mw:WikiLink\" href=\"/wiki/rabbit\" title=\"rabbit\">rabbit</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/rat\" title=\"rat\">rat</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/hedgehog\" title=\"hedgehog\">hedgehog</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/squirrel\" title=\"squirrel\">squirrel</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/fox\" title=\"fox\">fox</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/beaver\" title=\"beaver\">beaver</a>, <a rel=\"mw:WikiLink\" href=\"/wiki/badger\" title=\"badger\">badger</a>, etc."
            },
            {
              "definition": "A <a rel=\"mw:WikiLink\" href=\"/wiki/moth\" title=\"moth\">moth</a> of the genus <span class=\"biota\" about=\"#mwt35\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink/Interwiki\" href=\"https://species.wikimedia.org/wiki/Furcula\" title=\"wikispecies:Furcula\" class=\"extiw\"><i>Furcula</i></a><link rel=\"mw:PageProp/Category\" href=\"./Category:Entries_using_missing_taxonomic_name_(genus)#Furcula\"></span>."
            },
            {
              "definition": "<span class=\"usage-label-sense\" about=\"#mwt37\" typeof=\"mw:Transclusion\"></span> <span class=\"use-with-mention\" about=\"#mwt38\" typeof=\"mw:Transclusion\"><span class=\"Latn\" lang=\"en\">A <a rel=\"mw:WikiLink\" href=\"/wiki/term_of_endearment#English\" title=\"term of endearment\">term of endearment</a>, especially for a woman.</span></span>"
            }
          ]
        },
            {
              "partOfSpeech": "Verb",
              "language": "English",
              "definitions": [
                {
                  "definition": "to give <a rel=\"mw:WikiLink\" href=\"/wiki/birth\" title=\"birth\">birth</a> to kittens"
                }
              ]
            }
          ],
          "nl": [
            {
              "partOfSpeech": "Noun",
              "language": "Dutch",
              "definitions": [
                {
                  "definition": "a young <a rel=\"mw:WikiLink\" href=\"/wiki/cat\" title=\"cat\">cat</a>; <span class=\"Latn\" lang=\"en\" about=\"#mwt313\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink\" href=\"/wiki/kitten#English\" class=\"mw-selflink-fragment\">kitten</a></span>"
                }
              ]
            },
            {
              "partOfSpeech": "Verb",
              "language": "Dutch",
              "definitions": [
                {
                  "definition": "<span class=\"usage-label-sense\" about=\"#mwt320\" typeof=\"mw:Transclusion\"></span> to apply <a rel=\"mw:WikiLink\" href=\"/wiki/sealant\" title=\"sealant\">sealant</a> to"
                }
              ]
            },
            {
              "partOfSpeech": "Noun",
              "language": "Dutch",
              "definitions": [
                {
                  "definition": "<span class=\"form-of-definition use-with-mention\" about=\"#mwt327\" typeof=\"mw:Transclusion\"><a rel=\"mw:WikiLink\" href=\"/wiki/Appendix:Glossary#plural_number\" title=\"Appendix:Glossary\">plural</a> of <span class=\"form-of-definition-link\"><i class=\"Latn mention\" lang=\"nl\"><a rel=\"mw:WikiLink\" href=\"/wiki/kit#Dutch\" title=\"kit\">kit</a></i></span></span>"
                }
              ]
            }
          ],
          "de": [
            {
              "partOfSpeech": "Verb",
              "language": "German",
              "definitions": [
                {
                  "definition": "to <a rel=\"mw:WikiLink\" href=\"/wiki/putty\" title=\"putty\">putty</a>, to <a rel=\"mw:WikiLink\" href=\"/wiki/cement\" title=\"cement\">cement</a>"
                }
              ]
            }
          ]
        }
        """,
        []
    );

    public static readonly WiktionaryDefinitionsScenario KnownLanguage_UnknownPartOfSpeech = new(
        "xxx",
        Language.English, """
                          {
                              "en": [
                                  {
                                    "partOfSpeech": "xxx",
                                    "language": "English",
                                    "definitions": [
                                      {
                                        "definition": "definition_1"
                                      }
                                    ]
                                  }
                              ]
                          }
                          """,
        [
            new WordDefinition("xxx", Language.English, null, "definition_1")
        ]);

    public static readonly WiktionaryDefinitionsScenario UnknownLanguage_KnownLanguageRequested = new(
        "xxx",
        Language.English,
        """
        {
          "xxx": [ 
              {
                "partOfSpeech": "um, actually",
                "language": "Smut",
                "definitions": [
                  {
                    "definition": "definition_1"
                  }
                ]
              }
          ],
          "en": [
              {
                "partOfSpeech": "Noun",
                "language": "Ignore me",
                "definitions": [
                  {
                    "definition": "definition_1"
                  }
                ]
              }
            ]
        }
        """,
        [
            new WordDefinition("xxx", Language.English, PartOfSpeech.Noun, "definition_1")
        ]);

    public static readonly WiktionaryDefinitionsScenario LanguagePresent_WithoutDefinitions = new(
        nameof(LanguagePresent_WithoutDefinitions),
        Language.English,
        """
        {
          "en": []
        }
        """,
        []
    );
    
    public static IEnumerable<WiktionaryDefinitionsScenario> GetAllScenarios()
    {
        yield return LanguageNotPresent;
        yield return KnownLanguage_UnknownPartOfSpeech;
        yield return Kitten_English_Simplified;
        yield return Kitten_English;
        yield return Kitten_Dutch;
        yield return UnknownLanguage_KnownLanguageRequested;
        yield return LanguagePresent_WithoutDefinitions;
    }
}