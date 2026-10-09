using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Entity.Project;

public class ProjectEntity
{
    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}