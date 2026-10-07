using System.Net.Mime;
using Apps.BitbucketDataCenter.Api;
using Apps.BitbucketDataCenter.Extensions;
using Apps.BitbucketDataCenter.Models.Identifier;
using Apps.BitbucketDataCenter.Models.Identifier.Optional;
using Apps.BitbucketDataCenter.Models.Response.File;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.BitbucketDataCenter.Actions;

[ActionList("Files")]
public class FileActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : BitbucketInvocable(invocationContext)
{
    [Action("Download file", Description = "Download a file from")]
    public async Task<FileResponse> DownloadFile(
        [ActionParameter] ProjectIdentifier projectIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier,
        [ActionParameter] FileIdentifier fileIdentifier,
        [ActionParameter] OptionalBranchIdentifier branchIdentifier)
    {
        string endpoint = 
            $"projects/{projectIdentifier.ProjectKey}/repos/{repositoryIdentifier.RepositorySlug}" +
            $"/raw/{fileIdentifier.FilePath}";
        var request = new BitbucketRequest(endpoint).AddQueryParameterIfNotEmpty("at", branchIdentifier.BranchId);
        var response = await Client.ExecuteWithErrorHandling(request);
        
        byte[] bytes = response.RawBytes ?? 
                       throw new PluginMisconfigurationException("The downloaded file has no content");
        var stream = new MemoryStream(bytes);

        string fileName = response.GetFilenameFromDispositionHeader(fileIdentifier.FilePath);
        string contentType = response.ContentType ?? MediaTypeNames.Application.Octet;

        var file = await fileManagementClient.UploadAsync(stream, contentType, fileName);
        return new(file);
    }
}