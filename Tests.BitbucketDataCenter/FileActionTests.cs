using Apps.BitbucketDataCenter.Actions;
using Apps.BitbucketDataCenter.Models.Identifier;
using Apps.BitbucketDataCenter.Models.Identifier.Optional;
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
        var projectIdentifier = new ProjectIdentifier { ProjectKey = "AUT" };
        var repoIdentifier = new RepositoryIdentifier { RepositorySlug = "apidesigner" };
        var fileIdenfitier = new FileIdentifier { FilePath = "data/apidesigner.xls" };
        var branchIdentifier = new OptionalBranchIdentifier { BranchId = "" };

        // Act
        var result = await actions.DownloadFile(projectIdentifier, repoIdentifier, fileIdenfitier, branchIdentifier);

        // Assert
        Console.WriteLine(result.File.Name);
        Assert.IsNotNull(result.File);
    }
}