using Apps.BitbucketDataCenter.Api;
using Apps.BitbucketDataCenter.Models.Entity.Branch;
using Apps.BitbucketDataCenter.Models.Entity.Change;
using Apps.BitbucketDataCenter.Models.Entity.Commit;
using Apps.BitbucketDataCenter.Models.Utility.Pagination;
using RestSharp;

namespace Apps.BitbucketDataCenter.Extensions;

public static class BitbucketClientExtensions
{
    public static async Task<string> GetDefaultBranchId(this BitbucketClient client, string projectKey, string repositorySlug)
    {
        var request = new BitbucketRequest($"projects/{projectKey}/repos/{repositorySlug}/default-branch");
        var branch = await client.ExecuteWithErrorHandling<BranchEntity>(request);
        return branch.Id;
    }
    
    // https://developer.atlassian.com/server/bitbucket/rest/v1005/api-group-repository/#api-api-latest-projects-projectkey-repos-repositoryslug-branches-get
    public static Task<List<BranchEntity>> GetBranches(this BitbucketClient client, string projectKey, string repositorySlug)
    {
        return client.Paginate<BranchEntity>(new BitbucketRequest($"projects/{projectKey}/repos/{repositorySlug}/branches"));
    }
    
    // https://developer.atlassian.com/server/bitbucket/rest/v1005/api-group-repository/#api-api-latest-projects-projectkey-repos-repositoryslug-commits-get
    public static Task<List<CommitEntity>> GetCommitsBetween(
        this BitbucketClient client,
        string projectKey,
        string repositorySlug,
        string since,
        string until)
    {
        var request = new BitbucketRequest($"projects/{projectKey}/repos/{repositorySlug}/commits")
            .AddQueryParameter("since", since)
            .AddQueryParameter("until", until);
        return client.Paginate<CommitEntity>(request);
    }
    
    // https://developer.atlassian.com/server/bitbucket/rest/v1005/api-group-repository/#api-api-latest-projects-projectkey-repos-repositoryslug-commits-commitid-changes-get
    public static Task<List<ChangeEntity>> GetCommitChanges(
        this BitbucketClient client,
        string projectKey,
        string repositorySlug,
        string commitId)
    {
        var request = new BitbucketRequest($"projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/changes");
        return client.Paginate<ChangeEntity>(request);
    }
    
    public static async Task<string?> GetLastCommitId(
        this BitbucketClient client, 
        string projectKey, 
        string repositorySlug, 
        string filePath, 
        string branchId)
    {
        var request = new BitbucketRequest($"projects/{projectKey}/repos/{repositorySlug}/commits")
            .AddQueryParameter("path", filePath)
            .AddQueryParameter("until", branchId)
            .AddQueryParameter("limit", 1);

        var response = await client.ExecuteWithErrorHandling<PaginatedResponse<CommitEntity>>(request);
        return response.Values.FirstOrDefault()?.Id;
    }
}