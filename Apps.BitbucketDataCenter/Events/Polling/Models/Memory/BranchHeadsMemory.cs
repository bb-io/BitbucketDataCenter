namespace Apps.BitbucketDataCenter.Events.Polling.Models.Memory;

public class BranchHeadsMemory
{
    public DateTime LastPollingTime { get; set; }

    public Dictionary<string, Dictionary<string, string>> BranchHeads { get; set; } = [];
}