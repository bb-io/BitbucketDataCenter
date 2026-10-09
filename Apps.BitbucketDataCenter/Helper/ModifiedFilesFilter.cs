using Apps.BitbucketDataCenter.Events.Polling.Models.Request.File;
using Apps.BitbucketDataCenter.Models.Entity.Commit;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Apps.BitbucketDataCenter.Helper;

public class ModifiedFilesFilter(OnFilesModifiedAcrossReposRequest input)
{
    private readonly Matcher? _fileMatcher = BuildMatcher(input.FilePatternsToWatch);

    public bool IsBranchWatched(string branchName)
    {
        return IsAllowed(branchName, input.BranchesToWatch, input.BranchesToIgnore, IsEqual);
    }

    public bool IsCommitWatched(CommitEntity commit)
    {
        return 
            IsAllowed(commit.Message, input.CommitMessagesToInclude, input.CommitMessagesToExclude, IsContained) && 
            IsAllowed(commit.Author.EmailAddress, input.AuthorEmailsToWatch, input.AuthorEmailsToIgnore, IsEqual);
    }
    
    public bool IsFileWatched(string filePath) => _fileMatcher?.Match(filePath).HasMatches ?? true;

    private static bool IsAllowed(
        string value,
        List<string>? include,
        List<string>? exclude,
        Func<string, string, bool> matchSelector)
    {
        return (include?.Count == 0 || include?.Any(x => matchSelector(value, x)) == true) && 
               exclude?.Any(x => matchSelector(value, x)) != true;
    }
    
    private static bool IsEqual(string value, string pattern) => value.Equals(pattern, StringComparison.OrdinalIgnoreCase);

    private static bool IsContained(string value, string pattern) => value.Contains(pattern, StringComparison.OrdinalIgnoreCase);

    private static Matcher? BuildMatcher(List<string>? patterns)
    {
        if (patterns == null || patterns.Count == 0)
            return null;

        var matcher = new Matcher();
        matcher.AddIncludePatterns(patterns);
        return matcher;
    }
}