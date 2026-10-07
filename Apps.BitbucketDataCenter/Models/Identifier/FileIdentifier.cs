using Blackbird.Applications.Sdk.Common;

namespace Apps.BitbucketDataCenter.Models.Identifier;

public class FileIdentifier
{
    [Display("File path")]
    public string FilePath { get; set; } = string.Empty;
}