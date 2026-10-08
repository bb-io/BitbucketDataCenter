using Apps.BitbucketDataCenter.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.BitbucketDataCenter.Events.Polling.Models.Request.File;

public class OnFilesModifiedAcrossReposRequest
{
    [Display("Repository slugs to include"), DataSource(typeof(RepositoryDataHandler))]
    public List<string> RepositorySlugsToInclude { get; set; } = [];
    
    [Display("Branches to watch", Description = "All branches if empty"), DataSource(typeof(BranchDataHandler))]
    public List<string>? BranchesToWatch { get; set; }

    [Display("Branches to ignore"), DataSource(typeof(BranchDataHandler))]
    public List<string>? BranchesToIgnore { get; set; }

    [Display("File patterns to watch", Description = "For example, **/*.json or locales/en/*")]
    public List<string>? FilePatternsToWatch { get; set; }

    [Display("Commit messages to include", Description = "Commit message must contain one of these")]
    public List<string>? CommitMessagesToInclude { get; set; }

    [Display("Commit messages to exclude")]
    public List<string>? CommitMessagesToExclude { get; set; }

    [Display("Author emails to watch")]
    public List<string>? AuthorEmailsToWatch { get; set; }

    [Display("Author emails to ignore")]
    public List<string>? AuthorEmailsToIgnore { get; set; }
}