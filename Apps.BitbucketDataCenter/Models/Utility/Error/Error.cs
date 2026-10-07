using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Utility.Error;

public class Error
{
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
}