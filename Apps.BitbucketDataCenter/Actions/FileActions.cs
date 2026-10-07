using System.Net.Mime;
using Apps.BitbucketDataCenter.Api;
using Apps.BitbucketDataCenter.Constants;
using Apps.BitbucketDataCenter.Extensions;
using Apps.BitbucketDataCenter.Models.Identifier;
using Apps.BitbucketDataCenter.Models.Identifier.Optional;
using Apps.BitbucketDataCenter.Models.Request.File;
using Apps.BitbucketDataCenter.Models.Response.File;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Filters.Transformations;
using RestSharp;

namespace Apps.BitbucketDataCenter.Actions;

[ActionList("Files")]
public class FileActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : BitbucketInvocable(invocationContext)
{
    // https://developer.atlassian.com/server/bitbucket/rest/v1005/api-group-repository/#api-api-latest-projects-projectkey-repos-repositoryslug-raw-path-get
    [Action("Download file", Description = "Download a specific file from a repository")]
    public async Task<FileResponse> DownloadFile(
        [ActionParameter] ProjectIdentifier projectIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier,
        [ActionParameter] FileIdentifier fileIdentifier,
        [ActionParameter] OptionalBranchIdentifier branchIdentifier,
        [ActionParameter] DownloadFileRequest downloadInput)
    {
        string projectKey = projectIdentifier.ProjectKey;
        string repositorySlug = repositoryIdentifier.RepositorySlug;
        string filePath = fileIdentifier.FilePath;
        
        string endpoint = $"projects/{projectKey}/repos/{repositorySlug}/raw/{filePath}";
        var request = new BitbucketRequest(endpoint).AddQueryParameterIfNotEmpty("at", branchIdentifier.BranchId);
        var response = await Client.ExecuteWithErrorHandling(request);
        
        byte[] bytes = response.RawBytes ?? 
                       throw new PluginMisconfigurationException("The downloaded file has no content");
        var stream = new MemoryStream(bytes);

        string fileName = response.GetFilenameFromDispositionHeader(filePath);
        string contentType = response.ContentType ?? MediaTypeNames.Application.Octet;
        
        var fileResult = Transformation.Load(stream, fileName, contentType).Source();
        if (!fileResult.Success)
        {
            var directFileReference = await fileManagementClient.UploadAsync(stream, contentType, fileName);
            InvocationContext.Logger?.LogInformation($"Not a Blackbird interoperable file: {fileResult.Error}", []);
            return new(directFileReference);
        }

        var fileContent = fileResult.Value;
        
        fileContent.Language = downloadInput.SourceLanguage;
        fileContent.SystemReference.ContentId = downloadInput.ContentId;
        fileContent.SystemReference.AddMetadata(
            Creds.Get(CredsNames.InstanceUrl).Value.TrimEnd('/'), 
            projectKey, 
            repositorySlug, 
            branchIdentifier.BranchId, 
            filePath, 
            fileName);

        var file = await fileManagementClient.UploadAsync(fileContent.ToStream(), contentType, fileName);
        return new(file);
    }

    // https://developer.atlassian.com/server/bitbucket/rest/v1005/api-group-repository/#api-api-latest-projects-projectkey-repos-repositoryslug-browse-path-put
    [Action("Upload file", Description = "Commit a file upload. Overwrites existing files")]
    public async Task<FileResponse> UploadFile(
        [ActionParameter] ProjectIdentifier projectIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier,
        [ActionParameter] OptionalFileIdentifier fileIdentifier,
        [ActionParameter] OptionalBranchIdentifier branchIdentifier,
        [ActionParameter] UploadFileRequest uploadInput)
    {
        string projectKey = projectIdentifier.ProjectKey;
        string repositorySlug = repositoryIdentifier.RepositorySlug;
        string branchId = branchIdentifier.BranchId ?? await Client.GetDefaultBranchId(projectKey, repositorySlug);
        string filePath = string.IsNullOrWhiteSpace(fileIdentifier.FilePath)
            ? uploadInput.File.Name
            : fileIdentifier.FilePath;
        string fileName = Path.GetFileName(filePath);
        string? sourceCommitId = await Client.GetLastCommitId(projectKey, repositorySlug, filePath, branchId);

        await using var fileStream = await fileManagementClient.DownloadAsync(uploadInput.File);
        var transformationResult = Transformation.Load(fileStream, uploadInput.File.Name, uploadInput.File.ContentType);
        
        transformationResult.ReadContent(fileStream, InvocationContext.Logger);
        var fileBytes = await fileStream.GetByteData();

        string endpoint = $"projects/{projectKey}/repos/{repositorySlug}/browse/{filePath}";
        var request = new BitbucketRequest(endpoint, Method.Put) { AlwaysMultipartFormData = true }
            .AddParameter("branch", branchId)
            .AddFile("content", fileBytes, fileName)
            .AddParameter("message", uploadInput.CommitMessage ?? $"Upload {filePath}")
            .AddParameterIfNotEmpty("sourceCommitId", sourceCommitId);

        await Client.ExecuteWithErrorHandling(request);
        
        if (!transformationResult.Success) 
            return new(uploadInput.File);
        
        var transformation = transformationResult.Value;
        transformation.TargetSystemReference.AddMetadata(
            Creds.Get(CredsNames.InstanceUrl).Value.TrimEnd('/'), 
            projectKey, 
            repositorySlug, 
            branchIdentifier.BranchId, 
            filePath, 
            fileName);

        var fileData = transformationResult.ToResultFile();
        var uploadedFile = await fileManagementClient.UploadAsync(fileData.Stream, fileData.MediaType, fileData.FileName);
        return new(uploadedFile);
    }
}