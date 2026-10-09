using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Entity.Path;

public class PathValue
{
    [JsonProperty("toString")]
    public string FullPath { get; set; } = string.Empty;
}