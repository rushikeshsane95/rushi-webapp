using System.Net;

namespace Client.Tests.Support;

internal sealed class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly IReadOnlyDictionary<string, string> responses;

    public TestHttpMessageHandler(IReadOnlyDictionary<string, string> responses)
    {
        this.responses = responses;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.PathAndQuery.TrimStart('/') ?? string.Empty;

        if (responses.TryGetValue(path, out var content))
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            });
        }

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            RequestMessage = request
        });
    }
}
