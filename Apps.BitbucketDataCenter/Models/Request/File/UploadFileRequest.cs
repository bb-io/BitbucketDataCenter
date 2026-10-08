using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.BitbucketDataCenter.Models.Request.File;

public class UploadFileRequest
{
    [Display("File")]
    public FileReference File { get; set; } = null!;

    [Display("Commit message", Description = "Defaults to 'Upload {path}'")]
    public string? CommitMessage { get; set; }
}