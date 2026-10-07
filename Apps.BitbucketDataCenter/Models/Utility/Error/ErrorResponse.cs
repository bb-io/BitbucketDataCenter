using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Utility.Error;

public class ErrorResponse
{
    [JsonProperty("errors")]
    public List<Error> Errors { get; set; } = [];
}