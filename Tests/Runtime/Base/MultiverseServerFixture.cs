using System.Collections;
using Multiverse.Utils;

namespace Multiverse.Tests.Base
{
    public abstract class MultiverseServerFixture : MultiverseTestFixture
    {
        protected override IEnumerator UnityOneTimeSetUp()
        {
            yield return new WaitForTask(NetworkManager.Matchmaker.Connect());
            yield return new WaitForTask(HostServerMatch());

            StartTestClient();
            yield return new WaitUntilTimeout(() => NetworkManager.Client.Connections.Count > 1);
        }
    }
}