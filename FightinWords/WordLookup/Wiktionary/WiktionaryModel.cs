using System.Collections.Immutable;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace FightinWords.WordLookup.Wiktionary;

public static class WiktionaryModel
{
    /// <summary>
    /// The response from a <see cref="HttpMethod.Get"/> request to <a href="https://en.wiktionary.org/api/rest_v1/#/Page%20content/get_page_definition__term_">/page/definition/{word}</a>.
    /// </summary>
    public record DefinitionsResponse(ImmutableDictionary<string, ImmutableList<UsageDescription>> Usages)
    {
        [LanguageInjection("JSON")]
        public const string Example = $$"""
                                        {
                                          "en": [
                                            {{UsageDescription.Example}},
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
                                        """;

        public ImmutableList<UsageDescription> this[string isoTwoLetterLanguageCode] =>
            Usages.GetValueOrDefault(isoTwoLetterLanguageCode);

        public ImmutableList<UsageDescription> this[Language language] =>
            Usages.GetValueOrDefault(language.LanguageCode(), ImmutableList<UsageDescription>.Empty);
    }

    public readonly record struct UsageDescription(
        [property: JsonRequired] string PartOfSpeech,
        [property: JsonRequired] string Language,
        [property: JsonRequired] ImmutableList<DefinitionEntry> Definitions
    ) : IJsonOnDeserialized
    {
        [LanguageInjection("JSON")]
        public const string Example = $$"""
                                        {
                                          "partOfSpeech": "Noun",
                                          "language": "English",
                                          "definitions": [
                                            {{DefinitionEntry.Example}},
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
                                        }
                                        """;

        public void OnDeserialized()
        {
            ArgumentNullException.ThrowIfNull(PartOfSpeech);
            ArgumentNullException.ThrowIfNull(Language);
            ArgumentNullException.ThrowIfNull(Definitions);
        }
    }

    public readonly record struct DefinitionEntry([property: JsonRequired] string Definition) : IJsonOnDeserialized
    {
        [LanguageInjection("JSON")]
        public const string Example = """
                                      {
                                        "definition": "A <a rel=\"mw:WikiLink\" href=\"/wiki/young\" title=\"young\">young</a> <a rel=\"mw:WikiLink\" href=\"/wiki/cat\" title=\"cat\">cat</a>, especially before <a rel=\"mw:WikiLink\" href=\"/wiki/sexual\" title=\"sexual\">sexual</a> <a rel=\"mw:WikiLink\" href=\"/wiki/maturity\" title=\"maturity\">maturity</a> (reached at about seven months)."
                                      }
                                      """;

        void IJsonOnDeserialized.OnDeserialized()
        {
            ArgumentNullException.ThrowIfNull(Definition);
        }
    }
}