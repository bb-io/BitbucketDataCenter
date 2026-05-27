using Apps.BitbucketDataCenter.Actions;
using Tests.BitbucketDataCenter.Base;

namespace Tests.BitbucketDataCenter;

[TestClass]
public class ActionTests : TestBase
{
    [TestMethod]
    public async Task Dynamic_handler_works()
    {
        var actions = new Actions(InvocationContext);

        await actions.Action();
    }
}
