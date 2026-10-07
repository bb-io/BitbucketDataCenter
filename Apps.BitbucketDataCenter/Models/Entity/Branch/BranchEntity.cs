using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Entity.Branch;

public class BranchEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("displayId")]
    public string DisplayId { get; set; } = string.Empty;
}