using Apps.BitbucketDataCenter.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.BitbucketDataCenter.Models.Identifier;

public class RepositoryIdentifier
{
    [Display("Repository slug"), DataSource(typeof(RepositoryDataHandler))]
    public string RepositorySlug { get; set; } = string.Empty;
}