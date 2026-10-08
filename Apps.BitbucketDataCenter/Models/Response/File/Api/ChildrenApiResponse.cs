using Apps.BitbucketDataCenter.Models.Entity.Path;
using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Response.File.Api;

public class ChildrenApiResponse
{
    [JsonProperty("values")]
    public List<PathEntity> Values { get; set; } = [];
}