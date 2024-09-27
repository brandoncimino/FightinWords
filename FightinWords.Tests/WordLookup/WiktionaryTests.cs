using System.Collections.Immutable;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using FightinWords.WordLookup.Wiktionary;
using FluentAssertions;
using NUnit.Framework.Interfaces;

namespace FightinWords.WordLookup;

public class WiktionaryTests
{
    private static WiktionaryClient CreateWiktionaryClient(HttpStatusCode statusCode, string json)
    {
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content    = new StringContent(json, Encoding.UTF8, "application/json")
        };

        return new WiktionaryClient
        {
            HttpClient = new HttpClient(new MockHttpHandler([responseMessage]))
        };
    }

    [TestCase(WiktionaryModel.DefinitionsResponse.ExampleWord, WiktionaryModel.DefinitionsResponse.Example)]
    public void SendGetDefinitionsRequestAsync(string word, string responseJson)
    {
        var wiktionaryClient = CreateWiktionaryClient(HttpStatusCode.OK, responseJson);
        var expectedUsages   = JsonSerializer.Deserialize(responseJson, WiktionaryJsonContext.Default.UsageMap);

        var result1 = wiktionaryClient.SendDefinitionsRequestAsync(word).Result;

        result1.Usages.Should().BeEquivalentTo(expectedUsages);
        
        var result2 = wiktionaryClient.SendDefinitionsRequestAsync(word).Result;
        result2.Should().BeSameAs(result1,  $"a second request for the same word (\"{word}\") should return the EXACT same response object (without invoking the {nameof(WiktionaryClient.HttpClient)} a second time)");
    }

    [Test]
    public void SendGetDefinitionsRequestAsync_IsCached_Concurrently()
    {
        var wiktionaryClient = CreateWiktionaryClient(HttpStatusCode.OK, WiktionaryModel.DefinitionsResponse.Example);

        var responses = Enumerable.Repeat(WiktionaryModel.DefinitionsResponse.ExampleWord, 100)
                                  .AsParallel()
                                  .Select(it => wiktionaryClient.SendDefinitionsRequestAsync(it))
                                  .Select(it => it.Result)
                                  .ToImmutableList();

        var first = responses.First();
        responses.Should().AllSatisfy(it => it.Should().BeSameAs(first));
    }

    [Test]
    public void GetDefinitionsAsync([ValueSource(typeof(WiktionaryDefinitionsScenario), nameof(WiktionaryDefinitionsScenario.GetAllScenarios))] WiktionaryDefinitionsScenario scenario)
    {
        var wiktionaryClient = CreateWiktionaryClient(HttpStatusCode.OK, scenario.WiktionaryResponseJson);

        var actualDefinitions = wiktionaryClient.GetDefinitionsAsync(scenario.RequestedWord, scenario.RequestedLanguage)
                                                .Result.ToImmutableList();
        
        TestHelpers.AssertEquals(actualDefinitions, scenario.ExpectedDefinitions);
    }

    [Test]
    public void WordNotFound()
    {
        var client = CreateWiktionaryClient(HttpStatusCode.NotFound, "");

        var definitions = client.GetDefinitionsAsync(nameof(WordNotFound), Language.English).Result;
        Assert.That(definitions, Is.Empty);

        var secondDefinitions = client.GetDefinitionsAsync(nameof(WordNotFound), Language.English).Result;
        Assert.That(secondDefinitions, Is.Empty);
    }

    [Test]
    public void OtherHttpError()
    {
        var client = CreateWiktionaryClient(HttpStatusCode.EarlyHints, Guid.NewGuid().ToString());
        
        var definitions = client.GetDefinitionsAsync(nameof(OtherHttpError), Language.English);
        
        Assert.That(() => definitions.Result, Throws.Exception);
    }
}