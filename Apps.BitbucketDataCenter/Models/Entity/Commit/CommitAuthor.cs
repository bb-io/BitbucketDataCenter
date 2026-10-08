using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Entity.Commit;

public class CommitAuthor
{
    [JsonProperty("emailAddress")]
    public string EmailAddress { get; set; } = string.Empty;
}