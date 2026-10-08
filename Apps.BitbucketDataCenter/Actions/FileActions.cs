using System.Net.Mime;
using Apps.BitbucketDataCenter.Api;
using Apps.BitbucketDataCenter.Extensions;
using Apps.BitbucketDataCenter.Models.Identifier;
using Apps.BitbucketDataCenter.Models.Identifier.Optional;
using Apps.BitbucketDataCenter.Models.Request.File;
using Apps.BitbucketDataCenter.Models.Response.File;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
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

        string fileName = response.GetFilenameFromDispositionHeader(filePath);
        string contentType = response.ContentType ?? MediaTypeNames.Application.Octet;

        var stream = new MemoryStream(bytes);
        var fileResult = Transformation.Load(stream, fileName, contentType).Source();
        if (!fileResult.Success)
        {
            stream.Position = 0;
            var directFileReference = await fileManagementClient.UploadAsync(stream, contentType, fileName);
            InvocationContext.Logger?.LogInformation($"Not a Blackbird interoperable file: {fileResult.Error}", []);
            return new(directFileReference);
        }

        var fileContent = fileResult.Value;
        
        fileContent.Language = downloadInput.SourceLanguage;
        fileContent.SystemReference.ContentId = downloadInput.ContentId;
        fileContent.SystemReference.AddMetadata(
            InstanceUrl, 
            projectKey, 
            repositorySlug, 
            branchIdentifier.BranchId, 
            filePath, 
            fileName);

        var file = await fileManagementClient.UploadAsync(fileContent.ToStream(), contentType, fileName);
        return new(file);
    }

    [Action("Upload file", Description = "Commit a file upload - either create a new file or overwrite the existing one")]
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

        var file = await CommitFile(
            projectKey, 
            repositorySlug, 
            branchId, 
            uploadInput.File, 
            fileIdentifier.FilePath,
            folderPath: null,
            uploadInput.CommitMessage);
        return new(file);
    }
    
    [Action("Upload files", Description = "Commit multiple files, one commit per file. Creates new files or overwrites existing ones")]
    public async Task<UploadFilesResponse> UploadFiles(
        [ActionParameter] ProjectIdentifier projectIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier,
        [ActionParameter] OptionalBranchIdentifier branchIdentifier,
        [ActionParameter] OptionalFolderIdentifier folderIdentifier,
        [ActionParameter] UploadFilesRequest uploadInput)
    {
        string projectKey = projectIdentifier.ProjectKey;
        string repositorySlug = repositoryIdentifier.RepositorySlug;
        string branchId = branchIdentifier.BranchId ?? await Client.GetDefaultBranchId(projectKey, repositorySlug);

        // Sequential on purpose: each commit moves branch head so parallel requests would conflict
        var uploadedFiles = new List<FileReference>();
        foreach (var file in uploadInput.Files)
        {
            var commitedFile = await CommitFile(
                projectKey, 
                repositorySlug,
                branchId,
                file,
                explicitPath: null,
                folderIdentifier.FolderPath,
                uploadInput.CommitMessage);
            uploadedFiles.Add(commitedFile);
        }

        return new(uploadedFiles);
    }
    
    // https://developer.atlassian.com/server/bitbucket/rest/v1005/api-group-repository/#api-api-latest-projects-projectkey-repos-repositoryslug-browse-path-put
    private async Task<FileReference> CommitFile(
        string projectKey,
        string repositorySlug,
        string branchId,
        FileReference file,
        string? explicitPath,
        string? folderPath,
        string? commitMessage)
    {
        await using var fileStream = await fileManagementClient.DownloadAsync(file);
        var transformationResult = Transformation.Load(fileStream, file.Name, file.ContentType);
        fileStream.Position = 0;

        await using var contentStream = transformationResult.ReadContent(fileStream, InvocationContext.Logger);
        var targetResult = transformationResult.Success ? transformationResult.Value.Target() : null;

        string defaultName = targetResult is { Success: true } ? targetResult.Value.OriginalName : file.Name;
        string filePath = string.IsNullOrWhiteSpace(explicitPath)
            ? Path.Combine(folderPath?.Trim('/') ?? string.Empty, defaultName)
            : explicitPath;
        
        string fileName = Path.GetFileName(filePath);
        byte[] fileBytes = await contentStream.GetByteData();
        string? sourceCommitId = await Client.GetLastCommitId(projectKey, repositorySlug, filePath, branchId);

        string endpoint = $"projects/{projectKey}/repos/{repositorySlug}/browse/{filePath}";
        var request = new BitbucketRequest(endpoint, Method.Put) { AlwaysMultipartFormData = true }
            .AddParameter("branch", branchId)
            .AddFile("content", fileBytes, fileName)
            .AddParameter("message", commitMessage ?? $"Upload {filePath}")
            .AddParameterIfNotEmpty("sourceCommitId", sourceCommitId);

        await Client.ExecuteWithErrorHandling(request);

        if (!transformationResult.Success)
            return file;

        transformationResult.Value.TargetSystemReference.AddMetadata(
            InstanceUrl, 
            projectKey,
            repositorySlug,
            branchId,
            filePath,
            fileName);

        var fileData = transformationResult.ToResultFile();
        return await fileManagementClient.UploadAsync(fileData.Stream, fileData.MediaType, fileData.FileName);
    }
}