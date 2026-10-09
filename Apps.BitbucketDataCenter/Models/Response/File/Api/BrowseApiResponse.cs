using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Response.File.Api;

public class BrowseApiResponse
{
    [JsonProperty("children")]
    public ChildrenApiResponse Children { get; set; } = null!;
}