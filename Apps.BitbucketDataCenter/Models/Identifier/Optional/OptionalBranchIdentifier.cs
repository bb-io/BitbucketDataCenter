using Apps.BitbucketDataCenter.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.BitbucketDataCenter.Models.Identifier.Optional;

public class OptionalBranchIdentifier
{
    [Display("Display ID"), DataSource(typeof(BranchDataHandler))]
    public string? BranchId { get; set; }
}