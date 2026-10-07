using Blackbird.Filters.Shared;

namespace Apps.BitbucketDataCenter.Extensions;

public static class TransformationExtensions
{
    public static void AddMetadata(
        this SystemReference reference,
        string instanceUrl,
        string projectKey,
        string repositorySlug,
        string? branchName,
        string filePath,
        string fileName)
    {
        string baseUrl = instanceUrl.TrimEnd('/');
        string escapedPath = string.Join("/", filePath.Split('/').Select(Uri.EscapeDataString));
        
        reference.AdminUrl = $"{baseUrl}/projects/{projectKey}/repos/{repositorySlug}/browse/{escapedPath}";
        if (!string.IsNullOrWhiteSpace(branchName))
        {
            string escapedRef = Uri.EscapeDataString($"refs/heads/{branchName}");
            reference.AdminUrl += $"?at={escapedRef}";
        }

        reference.ContentName = fileName;
        reference.SystemName = "Bitbucket Data Center";
        reference.SystemRef = "https://bitbucket-data-center.org";  // Fake URL
    }
}