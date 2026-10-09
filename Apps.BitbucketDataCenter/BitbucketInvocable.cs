using Apps.BitbucketDataCenter.Api;
using Apps.BitbucketDataCenter.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;

namespace Apps.BitbucketDataCenter;

public class BitbucketInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds => InvocationContext.AuthenticationCredentialsProviders.ToArray();
    protected string InstanceUrl => Creds.Get(CredsNames.InstanceUrl).Value;
    protected BitbucketClient Client { get; }
    
    public BitbucketInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(Creds);
    }
}