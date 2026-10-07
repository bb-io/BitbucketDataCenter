using Apps.BitbucketDataCenter.Constants;
using Apps.BitbucketDataCenter.Models.Utility.Error;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.BitbucketDataCenter.Api;

public class BitbucketClient : BlackBirdRestClient
{
    public BitbucketClient(IEnumerable<AuthenticationCredentialsProvider> creds) : base(new()
    {
        BaseUrl = new Uri($"{creds.Get(CredsNames.InstanceUrl).Value.TrimEnd('/')}/rest/api/"),
    })
    {
        this.AddDefaultHeader("Authorization", $"Bearer {creds.Get(CredsNames.AccessToken).Value}");
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        string statusCodePart = $"Status code {response.StatusCode}.";
        string? responseContent = response.Content;
        
        if (string.IsNullOrWhiteSpace(responseContent))
            return new PluginApplicationException($"{statusCodePart} Server returned no content");

        ErrorResponse? errorResponse;
        try
        {
            errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
        }
        catch (JsonReaderException)
        {
            string rawErrorContent = responseContent.Substring(0, Math.Min(responseContent.Length, 300));
            return new PluginApplicationException($"{statusCodePart} Couldn't deserialize a JSON error. Raw: {rawErrorContent}");
        }

        if (errorResponse is null || errorResponse.Errors.Count == 0)
            return new PluginApplicationException($"{statusCodePart} An unknown error occured");
        
        var errorMessages = errorResponse.Errors.Select(x => x.Message);
        string errorMessage = string.Join("; ", errorMessages);
        return new PluginApplicationException(errorMessage);
    }
}