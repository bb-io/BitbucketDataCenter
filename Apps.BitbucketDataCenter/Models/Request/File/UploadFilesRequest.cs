using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.BitbucketDataCenter.Models.Request.File;

public class UploadFilesRequest
{
    [Display("Files")]
    public IEnumerable<FileReference> Files { get; set; } = [];

    [Display("Commit message", Description = "Used for every commit. Defaults to 'Upload {path}'")]
    public string? CommitMessage { get; set; }
}