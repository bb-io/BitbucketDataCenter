using Apps.BitbucketDataCenter.Api;
using Apps.BitbucketDataCenter.Extensions;
using Apps.BitbucketDataCenter.Models.Entity.Branch;
using Apps.BitbucketDataCenter.Models.Identifier;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.BitbucketDataCenter.Handlers;

public class BranchDataHandler : BitbucketInvocable, IAsyncDataSourceItemHandler
{
    private readonly string _projectKey;
    private readonly string _repositorySlug;
    
    public BranchDataHandler(
        InvocationContext invocationContext,
        [ActionParameter] ProjectIdentifier projectIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier) : base(invocationContext)
    {
        if (string.IsNullOrEmpty(projectIdentifier.ProjectKey) || string.IsNullOrEmpty(repositoryIdentifier.RepositorySlug))
            throw new PluginMisconfigurationException("Please specify a project key and a repository slug first");

        _projectKey = projectIdentifier.ProjectKey;
        _repositorySlug = repositoryIdentifier.RepositorySlug;
    }

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new BitbucketRequest($"projects/{_projectKey}/repos/{_repositorySlug}/branches")
            .AddQueryParameterIfNotEmpty("filterText", context.SearchString);

        var response = await Client.Paginate<BranchEntity>(request);
        return response.Select(x => new DataSourceItem(x.Id, x.DisplayId)).ToList();
    }
}