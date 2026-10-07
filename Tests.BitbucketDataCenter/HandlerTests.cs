using Apps.BitbucketDataCenter.Handlers;
using Apps.BitbucketDataCenter.Models.Identifier;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Tests.BitbucketDataCenter.Base;

namespace Tests.BitbucketDataCenter;

[TestClass]
public class HandlerTests : TestBase
{
    [TestMethod]
    public async Task ProjectDataHandler_ReturnsProjects()
    {
        // Arrange
        var handler = new ProjectDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task RepositoryDataHandler_ReturnsRepositories()
    {
        // Arrange
        var projectIdentifier = new ProjectIdentifier { ProjectKey = "AUT" };
        var handler = new RepositoryDataHandler(InvocationContext, projectIdentifier);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "API" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task FileDataHandler_ReturnsFiles()
    {
        // Arrange
        var projectIdentifier = new ProjectIdentifier { ProjectKey = "AUT" };
        var repoIdentifier = new RepositoryIdentifier { RepositorySlug = "apidesigner" };
        var handler = new FileDataHandler(InvocationContext, projectIdentifier, repoIdentifier);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task BranchDataHandler_ReturnsBranches()
    {
        // Arrange
        var projectIdentifier = new ProjectIdentifier { ProjectKey = "AUT" };
        var repoIdentifier = new RepositoryIdentifier { RepositorySlug = "apidesigner" };
        var handler = new BranchDataHandler(InvocationContext, projectIdentifier, repoIdentifier);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
}
