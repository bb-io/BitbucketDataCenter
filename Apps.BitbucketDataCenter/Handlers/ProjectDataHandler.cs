using Apps.BitbucketDataCenter.Api;
using Apps.BitbucketDataCenter.Extensions;
using Apps.BitbucketDataCenter.Models.Entity.Project;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.BitbucketDataCenter.Handlers;

public class ProjectDataHandler(InvocationContext invocationContext) 
    : BitbucketInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new BitbucketRequest("projects").AddQueryParameterIfNotEmpty("name", context.SearchString);
        var response = await Client.Paginate<ProjectEntity>(request, paginateTimes: 1);
        return response.Select(x => new DataSourceItem(x.Key, x.Name)).ToList();
    }
}
