using Apps.BitbucketDataCenter.Actions;
using Apps.BitbucketDataCenter.Models.Identifier;
using Apps.BitbucketDataCenter.Models.Identifier.Optional;
using Apps.BitbucketDataCenter.Models.Request.File;
using Tests.BitbucketDataCenter.Base;

namespace Tests.BitbucketDataCenter;

[TestClass]
public class FileActionTests : TestBase
{
    [TestMethod]
    public async Task DownloadFile_IsSuccess()
    {
        // Arrange
        var actions = new FileActions(InvocationContext, FileManager);
        var projectIdentifier = new ProjectIdentifier { ProjectKey = "" };
        var repoIdentifier = new RepositoryIdentifier { RepositorySlug = "" };
        var fileIdenfitier = new FileIdentifier { FilePath = "" };
        var branchIdentifier = new OptionalBranchIdentifier { BranchId = "" };
        var downloadInput = new DownloadFileRequest { ContentId = "test12345", SourceLanguage = "uk-UA" };

        // Act
        var result = await actions.DownloadFile(projectIdentifier, repoIdentifier, fileIdenfitier, branchIdentifier, downloadInput);

        // Assert
        Console.WriteLine(result.File.Name);
        Assert.IsNotNull(result.File);
    }
}