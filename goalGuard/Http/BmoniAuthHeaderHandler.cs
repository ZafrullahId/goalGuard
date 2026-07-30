using goalGuard.Contracts;
using Microsoft.Extensions.Options;

namespace goalGuard.Http;

public class BmoniAuthHeaderHandler : DelegatingHandler
{
    private readonly BmoniOptions _options;

    public BmoniAuthHeaderHandler(IOptions<BmoniOptions> options)
    {
        _options = options.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("x-api-key", _options.ApiKey);
        return await base.SendAsync(request, cancellationToken);
    }
}
