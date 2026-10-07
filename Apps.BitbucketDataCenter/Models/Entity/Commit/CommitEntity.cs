using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Entity.Commit;

public class CommitEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
}