using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Entity.Repository;

public class RepositoryEntity
{
    [JsonProperty("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}