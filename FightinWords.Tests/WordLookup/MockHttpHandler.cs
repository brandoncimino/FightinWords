using System.Collections.Immutable;

namespace FightinWords.WordLookup;

/// <summary>
/// A <b><i>very</i></b> simple <see cref="HttpResponseMessage"/> implementation that will return the given <see cref="myResponses"/>s
/// exactly once each, in order.
/// </summary>
/// <param name="myResponses">the <see cref="SendAsync"/>s that will be returned by <see cref="HttpMessageHandler"/></param>
public class MockHttpHandler(ImmutableList<HttpResponseMessage> myResponses) : HttpMessageHandler
{
    private int _messagesReturned;
    
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Assert.That(myResponses, Has.Count.AtLeast(_messagesReturned), $"I've already returned all {myResponses.Count} of my messages!");
        var toReturn = myResponses[_messagesReturned];
        _messagesReturned += 1;
        return Task.FromResult(toReturn);
    }
}