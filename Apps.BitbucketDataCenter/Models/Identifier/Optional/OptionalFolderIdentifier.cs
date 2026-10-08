using Apps.BitbucketDataCenter.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.BitbucketDataCenter.Models.Identifier.Optional;

public class OptionalFolderIdentifier
{
    [Display("Folder path"), DataSource(typeof(FolderDataHandler))]
    public string? FolderPath { get; set; }
}