using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Json;

namespace FightinWords.WordLookup.Wiktionary;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Online documentation: <a href="https://en.wiktionary.org/api/rest_v1/#/Page%20content/get_page_definition__term_">GET <c>page/definition/{term}</c></a></remarks>
public sealed partial class WiktionaryClient : IWordLookup
{
    public const string DefinitionsEndpoint = "page/definition";
    public       string WiktionaryDomain { get; init; } = "wiktionary.org";

    /// <summary>
    /// The language of <b><i>Wiktionary itself</i></b> - i.e. the language that the <see cref="WiktionaryModel.DefinitionEntry.Definition"/>s will be <i><b>written in</b></i>.
    /// You might still receive definitions that <i><b>apply</b></i> to other <see cref="WiktionaryModel.UsageDescription.Language"/>s.
    /// </summary>
    /// <example>
    /// Requesting <see cref="WiktionaryModel.DefinitionsResponse.ExampleWord">"kitten"</see> will return <see cref="WiktionaryModel.DefinitionsResponse.Example"/>,
    /// which is written in <see cref="Language.English"/> but contains a <see cref="WiktionaryModel.UsageDescription"/> for <see cref="Language.Dutch"/>.
    /// </example>
    public Language WiktionaryLanguage { get; init; } = Language.English;

    public HttpClient HttpClient { private get; init; } = new();

    public Uri? OverrideUrl;

    private static Uri GetWordEndpoint(string word, Language wiktionaryLanguage, string wiktionaryDomain)
    {
        //TODO: Should I be using some fancy builder or something that screens the `{word}` parameter for hax?
        return new Uri(
            $"https://{wiktionaryLanguage.IsoLanguageCode()}.{wiktionaryDomain}/api/rest_v1/{DefinitionsEndpoint}/{word}");
    }

    private readonly ConcurrentDictionary<string, Lazy<Task<WiktionaryModel.DefinitionsResponse>>> _taskCache = new();

    /// <summary>
    /// "Eagerly" starts up an asynchronous request to the Wiktionary <see cref="DefinitionsEndpoint"/>.
    /// </summary>
    /// <param name="word"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static async Task<WiktionaryModel.DefinitionsResponse> InvokeWiktionaryAsync(
        string           word,
        WiktionaryClient client
    )
    {
        var response = await client.HttpClient.GetAsync(client.OverrideUrl ??
                                                        GetWordEndpoint(word, client.WiktionaryLanguage,
                                                            client.WiktionaryDomain));

//         Console.WriteLine($"""
//                            RESPONSE for `{word}`: {response.StatusCode} "{response.ReasonPhrase}"
//                            ----
//                            {response.Content.ReadAsStringAsync().Result}
//                            ----
//                            """);

        // If the word wasn't found, return an empty usage map instead
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new WiktionaryModel.DefinitionsResponse(
                ImmutableDictionary<string, ImmutableList<WiktionaryModel.UsageDescription>>.Empty);
        }

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception($"Bad response code from Wiktionary: {response.StatusCode}; {response}");
        }

        var usageMap = await response
                             .Content
                             .ReadFromJsonAsync(WiktionaryJsonContext.Default.UsageMap);

        return new WiktionaryModel.DefinitionsResponse(usageMap.OrEmpty());
    }

    /// <summary>
    /// Sends request to the <see cref="DefinitionsEndpoint"/> and returns the raw <see cref="WiktionaryModel.DefinitionsResponse"/>.
    /// </summary>
    /// <param name="word">The word you want to look up</param>
    /// <returns>The corresponding <see cref="WiktionaryModel.DefinitionsResponse"/></returns>
    public async Task<WiktionaryModel.DefinitionsResponse> SendDefinitionsRequestAsync(
        string word
    )
    {
        var task = _taskCache.GetOrAdd(
            word,
            static (newWord, client) =>
                new Lazy<Task<WiktionaryModel.DefinitionsResponse>>(() => InvokeWiktionaryAsync(newWord, client)),
            this
        ).Value;

        return await task;
    }

    public async Task<IEnumerable<WordDefinition>> GetDefinitionsAsync(string word, Language language)
    {
        var response = await SendDefinitionsRequestAsync(word);
        return response.GetDefinitionsForLanguage(language)
                       .SelectMany(it =>
                       {
                           return it.Definitions
                                    .Select(def => new WordDefinition(
                                        word,
                                        language,
                                        it.MaybeGetPartOfSpeech(),
                                        def.Definition
                                    ));
                       });
    }
}