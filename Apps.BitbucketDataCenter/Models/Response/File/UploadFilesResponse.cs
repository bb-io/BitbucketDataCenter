using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.BitbucketDataCenter.Models.Response.File;

public record UploadFilesResponse(List<FileReference> Files);