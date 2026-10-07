using Apps.BitbucketDataCenter.Api;
using Apps.BitbucketDataCenter.Models.Entity.Branch;
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