using Apps.BitbucketDataCenter.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.BitbucketDataCenter.Models.Identifier;

public class FileIdentifier
{
    [Display("File path"), DataSource(typeof(FileDataHandler))]
    public string FilePath { get; set; } = string.Empty;
}