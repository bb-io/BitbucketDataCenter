using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.BitbucketDataCenter.Actions;

[ActionList]
public class Actions(InvocationContext invocationContext) : BitbucketInvocable(invocationContext)
{
    [Action("Action", Description = "Describes the action")]
    public async Task Action()
    {
        
    }
}