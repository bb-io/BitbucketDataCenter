using Apps.BitbucketDataCenter.Api;
using Apps.BitbucketDataCenter.Extensions;
using Apps.BitbucketDataCenter.Models.Entity.Repository;
using Apps.BitbucketDataCenter.Models.Identifier;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.BitbucketDataCenter.Handlers;

public class RepositoryDataHandler : BitbucketInvocable, IAsyncDataSourceItemHandler
{
    private readonly string _projectKey;
    
    public RepositoryDataHandler(
        InvocationContext invocationContext, 
        [ActionParameter] ProjectIdentifier projectIdentifier) : base(invocationContext)
    {
        if (string.IsNullOrEmpty(projectIdentifier.ProjectKey))
            throw new PluginMisconfigurationException("Please specify a project key first");

        _projectKey = projectIdentifier.ProjectKey;
    }

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new BitbucketRequest("repos")
            .AddQueryParameter("projectkey", _projectKey)
            .AddQueryParameterIfNotEmpty("name", context.SearchString);
        var result = await Client.Paginate<RepositoryEntity>(request, paginateTimes: 1);
        return result.Select(x => new DataSourceItem(x.Slug, x.Name)).ToList();
    }
}