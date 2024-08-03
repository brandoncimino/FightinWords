using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using FightinWords.WordLookup;
using FightinWords.WordLookup.Wiktionary;

namespace FightinWords;

public class Tests
{
    [Test]
    public void GetDefinitions_ForRealsies()
    {
        var realWiktionary = new WiktionaryClient();
        var kittens = realWiktionary.GetDefinitionsAsync("kittens").Result;
        Console.WriteLine(kittens);
    }
    
    [Test]
    public void GetDefinitions()
    {
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(WiktionaryModel.DefinitionsResponse.Example, Encoding.UTF8, "application/json")
        };
        
        var wiktionaryClient = new WiktionaryClient
        {
            HttpClient = new HttpClient(new MockHttpHandler([responseMessage]))
            {
                BaseAddress = new Uri(WiktionaryClient.DefaultUrl)
            }
        };

        var word = "kitten";
        var result = wiktionaryClient.GetDefinitionsAsync(word).Result;

        var resultNode = JsonSerializer.SerializeToNode(result.Usages, WiktionaryJsonContext.Default.Mutable_DefinitionsResponse);
        var expectedNode = JsonNode.Parse(WiktionaryModel.DefinitionsResponse.Example);
        
        Assert.That(JsonNode.DeepEquals(resultNode, expectedNode), "The result from the client should be equivalent to the original JSON");
        
        var result2 = wiktionaryClient.GetDefinitionsAsync(word).Result;
        Assert.That(result2, Is.SameAs(result), $"A second request for the same word (\"{word}\") should return the EXACT same response object (without invoking the {nameof(WiktionaryClient.HttpClient)} a second time)");
    }
}