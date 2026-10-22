using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.BitbucketDataCenter.Models.Response.File;

public record UploadFileResponse(
    FileReference File,
    [property: Display("Commit ID")] string CommitId,
    [property: Display("Commit URL")] string CommitUrl);
