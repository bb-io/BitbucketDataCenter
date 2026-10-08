using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Entity.Commit;

public class CommitEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("author")]
    public CommitAuthor Author { get; set; } = null!;
}