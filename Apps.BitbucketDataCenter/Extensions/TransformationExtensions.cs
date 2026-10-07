using Apps.BitbucketDataCenter.Models.Utility.File;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Filters.Constants;
using Blackbird.Filters.Enums;
using Blackbird.Filters.Shared;
using Blackbird.Filters.Transformations;

namespace Apps.BitbucketDataCenter.Extensions;

public static class TransformationExtensions
{
    public static void AddMetadata(
        this SystemReference reference,
        string instanceUrl,
        string projectKey,
        string repositorySlug,
        string? branchName,
        string filePath,
        string fileName)
    {
        string baseUrl = instanceUrl.TrimEnd('/');
        string escapedPath = string.Join("/", filePath.Split('/').Select(Uri.EscapeDataString));
        
        reference.AdminUrl = $"{baseUrl}/projects/{projectKey}/repos/{repositorySlug}/browse/{escapedPath}";
        if (!string.IsNullOrWhiteSpace(branchName))
        {
            string escapedRef = Uri.EscapeDataString(branchName);
            reference.AdminUrl += $"?at={escapedRef}";
        }

        reference.ContentName = fileName;
        reference.SystemName = "Bitbucket Data Center";
        reference.SystemRef = "https://bitbucket-data-center.org";  // Fake URL
    }
    
    public static ProcessedFile ToResultFile(this TransformationLoadResult transformationResult)
    {
        var transformation = transformationResult.Value ?? 
                             throw new InvalidOperationException("Transformation is empty");
        
        if (transformationResult.WasBilingual)
            return new(transformation.ToStream(), MediaTypes.Xliff2, transformation.BilingualFileName);
    
        var targetResult = transformation.Target();
        if (!targetResult.Success)
            throw new PluginMisconfigurationException(targetResult.Error);
    
        var target = targetResult.Value;
        target.SystemReference = transformation.TargetSystemReference;
        return new(target.ToStream(), target.OriginalMediaType, target.OriginalName);
    }

    public static Stream ReadContent(this TransformationLoadResult transformationResult, Stream fileStream, Logger? logger)
    {
        var contentResult = transformationResult.Target();
        if (contentResult.Success)
            return contentResult.Value.ToStream(MetadataHandling.Exclude);

        logger?.LogInformation($"Not a Blackbird interoperable file: {transformationResult.Error}", []);
        return fileStream;
    }
}