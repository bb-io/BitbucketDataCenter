using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.BitbucketDataCenter;

public class Application : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.SoftwareDevelopment];
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}