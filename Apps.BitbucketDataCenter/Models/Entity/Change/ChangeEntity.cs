using Apps.BitbucketDataCenter.Models.Entity.Path;
using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Entity.Change;

public class ChangeEntity
{
    [JsonProperty("path")]
    public PathValue Path { get; set; } = new();

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    public bool IsDeleted => Type.Equals("DELETE", StringComparison.OrdinalIgnoreCase);
}