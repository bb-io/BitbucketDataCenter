using Blackbird.Applications.Sdk.Common;

namespace Apps.BitbucketDataCenter.Models.Response.File;

public record CommitResponse(
    [property: Display("Commit ID")] string CommitId,
    [property: Display("Commit URL")] string CommitUrl);
