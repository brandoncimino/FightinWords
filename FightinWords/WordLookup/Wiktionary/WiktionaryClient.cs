using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Json;
using JetBrains.Annotations;

namespace FightinWords.WordLookup.Wiktionary;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Online documentation: <a href="https://en.wiktionary.org/api/rest_v1/#/Page%20content/get_page_definition__term_">GET <c>page/definition/{term}</c></a></remarks>
public sealed class WiktionaryClient : IWordLookup
{
    public const string DefaultUrl         = "https://en.wiktionary.org/api/rest_v1/";
    public const string DefinitionEndpoint = "page/definition";

    public HttpClient HttpClient { private get; init; } = new()
                                                          {
                                                              BaseAddress = new Uri(DefaultUrl)
                                                          };

    private readonly ConcurrentDictionary<string, Task<WiktionaryModel.DefinitionsResponse>> _responseCache = new();

    private static string GetWordEndpoint(string word)
    {
        //TODO: Should I be using some fancy builder or something that screens the `{word}` parameter for hax?
        return $"{DefinitionEndpoint}/{word}";
    }

    public Task<WiktionaryModel.DefinitionsResponse> GetDefinitionsAsync(string word)
    {
        return _responseCache.GetOrAdd(
                                       word,
                                       static async (w, client) =>
                                       {
                                           var response = await client.HttpClient.GetAsync(GetWordEndpoint(w));

                                           // If the word wasn't found, return an empty usage map instead
                                           if (response.StatusCode == HttpStatusCode.NotFound)
                                           {
                                               return new WiktionaryModel.DefinitionsResponse(ImmutableDictionary<string
                                                       ,
                                                       ImmutableList<WiktionaryModel.
                                                           UsageDescription>>
                                                   .Empty);
                                           }

                                           var usageMap =
                                               await response.Content.ReadFromJsonAsync(WiktionaryJsonContext.Default
                                                   .Mutable_DefinitionsResponse);

                                           return new WiktionaryModel.DefinitionsResponse(usageMap ??
                                               ImmutableDictionary<string,
                                                   ImmutableList<WiktionaryModel.UsageDescription>>.Empty);
                                       },
                                       this
                                      );
    }

    public async Task<bool> IsWordAsync(string word, Language language)
    {
        var definitionsResponse = await GetDefinitionsAsync(word);
        return definitionsResponse[language].IsEmpty;
    }

    public async Task<IEnumerable<WordDefinition>> GetDefinitionsAsync(string word, Language language)
    {
        var response = await GetDefinitionsAsync(word);
        return response[language]
            .SelectMany(it =>
            {
                if (Enum.TryParse<PartOfSpeech>(it.PartOfSpeech, out var partOfSpeech) == false)
                {
                    throw new ArgumentException($"Unknown {nameof(PartOfSpeech)}: {it.PartOfSpeech}");
                }
                
                return it.Definitions
                         .Select(def => new WordDefinition(
                                                           word,
                                                           language,
                                                           partOfSpeech,
                                                           def.Definition
                                                          ));
            });
    }

    public string GetRandomWord([ValueRange(1, int.MaxValue)] int length, Language language)
    {
        return length switch
               {
                   < 0 => throw new ArgumentOutOfRangeException(nameof(length), length, null),
                   1   => "a",
                   2   => "hi",
                   3   => "nip",
                   4   => "yolo",
                   5   => "dunce",
                   6   => "nipple",
                   7   => "extreme",
                   _ => throw new ArgumentOutOfRangeException(nameof(length), length, null)
               };
    }
}