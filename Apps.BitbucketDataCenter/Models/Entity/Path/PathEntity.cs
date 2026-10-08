using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Entity.Path;

public class PathEntity
{
    [JsonProperty("path")]
    public PathValue Path { get; set; } = null!;
    
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;
    
    public bool IsFolder => string.Equals(Type, "DIRECTORY", StringComparison.OrdinalIgnoreCase);
}