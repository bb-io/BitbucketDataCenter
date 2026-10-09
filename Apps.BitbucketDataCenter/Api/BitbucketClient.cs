using Apps.BitbucketDataCenter.Constants;
using Apps.BitbucketDataCenter.Models.Utility.Error;
using Apps.BitbucketDataCenter.Models.Utility.Pagination;
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

    // https://developer.atlassian.com/server/bitbucket/rest/v1005/intro/#paged-apis
    public async Task<List<T>> Paginate<T>(RestRequest request, int? paginateTimes = null)
    {
        int limit = 100;
        int? nextPageStart = 0;
        int timesPaginated = 0;
        
        List<T> resultValues = [];

        request.AddOrUpdateParameter("limit", limit);
        
        while (true)
        {
            if (paginateTimes == timesPaginated)
                break;
            
            request.AddOrUpdateParameter("start", nextPageStart!.Value);
            
            var paginatedResult = await ExecuteWithErrorHandling<PaginatedResponse<T>>(request);
            resultValues.AddRange(paginatedResult.Values);

            timesPaginated++;
            if (paginatedResult.IsLastPage)
                break;

            if (paginatedResult.NextPageStart is null)
                break;
            
            nextPageStart = paginatedResult.NextPageStart;
        }

        return resultValues;
    }

    // https://developer.atlassian.com/server/bitbucket/rest/v1005/intro/#errors---validation
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