using Apps.BitbucketDataCenter.Events.Polling.Models.Memory;
using Apps.BitbucketDataCenter.Events.Polling.Models.Request.File;
using Apps.BitbucketDataCenter.Extensions;
using Apps.BitbucketDataCenter.Helper;
using Apps.BitbucketDataCenter.Models.Entity.Branch;
using Apps.BitbucketDataCenter.Models.Identifier;
using Apps.BitbucketDataCenter.Models.Response.File;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.BitbucketDataCenter.Events.Polling;

[PollingEventList("Files")]
public class FilePollingList(InvocationContext invocationContext) : BitbucketInvocable(invocationContext)
{
    [MultipleEvents]
    [PollingEvent("On files modified across repositories", 
        Description = "Triggered when files across specific repositories of one project are modified")]
    public async Task<PollingEventResponse<BranchHeadsMemory, List<ModifiedFileResponse>>> OnFilesModifiedAcrossRepos(
        PollingEventRequest<BranchHeadsMemory> request,
        [PollingEventParameter] ProjectIdentifier projectIdentifier,
        [PollingEventParameter] OnFilesModifiedAcrossReposRequest input)
    {
        string projectKey = projectIdentifier.ProjectKey;
        
        var filter = new ModifiedFilesFilter(input);
        var currentHeads = new Dictionary<string, Dictionary<string, string>>();
        var modifiedFiles = new List<ModifiedFileResponse>();
        
        foreach (string repositorySlug in input.RepositorySlugsToInclude)
        {
            var branches = await Client.GetBranches(projectKey, repositorySlug);
            var watchedBranches = branches.Where(x => filter.IsBranchWatched(x.DisplayId)).ToList();
            currentHeads[repositorySlug] = watchedBranches.ToDictionary(x => x.Id, x => x.LatestCommit);
            
            if (request.Memory is null)
                continue;
            
            foreach (var branch in watchedBranches)
            {
                string? lastHead = request.Memory.BranchHeads.GetValueOrDefault(repositorySlug)?.GetValueOrDefault(branch.Id);
                if (lastHead is null || lastHead == branch.LatestCommit)
                    continue;

                var fetchedModifiedFiles = await GetModifiedFiles(projectKey, repositorySlug, branch, lastHead, filter);
                modifiedFiles.AddRange(fetchedModifiedFiles);
            }
        }
        
        var files = modifiedFiles.DistinctBy(x => (x.RepositorySlug, x.BranchId, x.FilePath)).ToList();
        return new()
        {
            FlyBird = files.Count > 0,
            Memory = new() { LastPollingTime = DateTime.UtcNow, BranchHeads = currentHeads },
            Result = new(files)
        };
    }
    
    private async Task<List<ModifiedFileResponse>> GetModifiedFiles(
        string projectKey,
        string repositorySlug,
        BranchEntity branch,
        string sinceCommit,
        ModifiedFilesFilter filter)
    {
        try
        {
            var commits = await Client.GetCommitsBetween(projectKey, repositorySlug, sinceCommit, branch.LatestCommit);
            var result = new List<ModifiedFileResponse>();

            foreach (var commit in commits.Where(filter.IsCommitWatched))
            {
                var changes = await Client.GetCommitChanges(projectKey, repositorySlug, commit.Id);
                result.AddRange(changes
                    .Where(x => !x.IsDeleted && filter.IsFileWatched(x.Path.FullPath))
                    .Select(x => new ModifiedFileResponse(projectKey, repositorySlug, branch.Id, x.Path.FullPath, commit.Id)));
            }

            return result;
        }
        catch (PluginApplicationException ex)
        {
            InvocationContext.Logger?.LogInformation($"Skipping {projectKey}/{repositorySlug} {branch.Id}: {ex.Message}", []);
            return [];
        }
    }
}