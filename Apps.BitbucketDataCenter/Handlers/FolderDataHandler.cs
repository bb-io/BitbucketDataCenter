using Apps.BitbucketDataCenter.Api;
using Apps.BitbucketDataCenter.Models.Identifier;
using Apps.BitbucketDataCenter.Models.Response.File.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.BitbucketDataCenter.Handlers;

public class FolderDataHandler : BitbucketInvocable, IAsyncDataSourceItemHandler
{
    private readonly string _projectKey;
    private readonly string _repositorySlug;
    
    public FolderDataHandler(
        InvocationContext invocationContext,
        [ActionParameter] ProjectIdentifier projectIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier) : base(invocationContext)
    {
        if (string.IsNullOrEmpty(projectIdentifier.ProjectKey) || string.IsNullOrEmpty(repositoryIdentifier.RepositorySlug))
            throw new PluginMisconfigurationException("Please specify a project key and a repository slug first");

        _projectKey = projectIdentifier.ProjectKey;
        _repositorySlug = repositoryIdentifier.RepositorySlug;
    }

    // https://developer.atlassian.com/server/bitbucket/rest/v1005/api-group-repository/#api-api-latest-projects-projectkey-repos-repositoryslug-browse-get
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new BitbucketRequest($"projects/{_projectKey}/repos/{_repositorySlug}/browse");
        var response = await Client.ExecuteWithErrorHandling<BrowseApiResponse>(request);
        return response.Children.Values
            .Where(x => x.IsFolder)
            .Select(x => x.Path.FullPath)
            .Where(x =>
                string.IsNullOrWhiteSpace(context.SearchString) ||
                x.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .Select(x => new DataSourceItem(x, x))
            .ToList();
    }
}