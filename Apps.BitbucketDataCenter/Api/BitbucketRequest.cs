using Apps.BitbucketDataCenter.Constants;
using RestSharp;

namespace Apps.BitbucketDataCenter.Api;

public class BitbucketRequest(string resource, Method method = Method.Get, string apiVersion = ApiVersion.Latest)
    : RestRequest($"{apiVersion}/{resource.TrimStart('/')}", method);