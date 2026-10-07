using RestSharp;

namespace Apps.BitbucketDataCenter.Extensions;

public static class RestRequestExtensions
{
    public static RestRequest AddQueryParameterIfNotEmpty(this RestRequest request, string key, string? value)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            return request;

        request.AddQueryParameter(key, value);
        return request;
    }
}