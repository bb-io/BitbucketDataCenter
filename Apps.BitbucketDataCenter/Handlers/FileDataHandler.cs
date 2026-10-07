using Apps.BitbucketDataCenter.Api;
using Apps.BitbucketDataCenter.Models.Identifier;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.BitbucketDataCenter.Handlers;

public class FileDataHandler : BitbucketInvocable, IAsyncDataSourceItemHandler
{
    private readonly string _projectKey;
    private readonly string _repositorySlug;
    
    public FileDataHandler(
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
        var request = new BitbucketRequest($"projects/{_projectKey}/repos/{_repositorySlug}/files");
        var result = await Client.Paginate<string>(request, paginateTimes: 2);
        return result
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString) || x.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x, x))
            .ToList();
    }
}