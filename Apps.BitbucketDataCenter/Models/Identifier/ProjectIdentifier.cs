using Apps.BitbucketDataCenter.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.BitbucketDataCenter.Models.Identifier;

public class ProjectIdentifier
{
    [Display("Project key"), DataSource(typeof(ProjectDataHandler))]
    public string ProjectKey { get; set; } = string.Empty;
}