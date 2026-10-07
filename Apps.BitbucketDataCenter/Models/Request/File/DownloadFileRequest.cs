using Blackbird.Applications.Sdk.Common;

namespace Apps.BitbucketDataCenter.Models.Request.File;

public class DownloadFileRequest
{
    [Display("Source language code")]
    public string? SourceLanguage { get; set; }

    [Display("Content ID", Description = "The ID of the content. Used by Blacklake when diffing")]
    public string? ContentId { get; set; }
}