using Newtonsoft.Json;

namespace Apps.BitbucketDataCenter.Models.Utility.Pagination;

public class PaginatedResponse<T>
{
    [JsonProperty("size")]
    public int Size { get; set; }
    
    [JsonProperty("limit")]
    public int Limit { get; set; }
    
    [JsonProperty("isLastPage")]
    public bool IsLastPage { get; set; }

    [JsonProperty("values")]
    public List<T> Values { get; set; } = [];

    [JsonProperty("nextPageStart")]
    public int? NextPageStart { get; set; }
}