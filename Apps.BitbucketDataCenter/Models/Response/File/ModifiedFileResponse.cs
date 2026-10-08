using Blackbird.Applications.Sdk.Common;

namespace Apps.BitbucketDataCenter.Models.Response.File;

public record ModifiedFileResponse(
    [property: Display("Project key")] string ProjectKey,
    [property: Display("Repository slug")] string RepositorySlug,
    [property: Display("Branch ID")] string BranchId,
    [property: Display("File path")] string FilePath,
    [property: Display("Commit ID")] string CommitId);