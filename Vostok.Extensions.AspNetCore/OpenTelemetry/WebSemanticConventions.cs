namespace Vostok.Extensions.AspNetCore.OpenTelemetry;

internal static class WebSemanticConventions
{
    public const string AttributeHttpRequestContentLength = "http.request.header.content-length";
    public const string AttributeHttpResponseContentLength = "http.response.header.content-length";
    public const string AttributeClientAddress = "client.address";
    public const string AttributeServerAddress = "server.address";
    public const string AttributeServerPort = "server.port";

    // Vostok tracing attributes
    public const string HttpClientName = "http.client.name";
}