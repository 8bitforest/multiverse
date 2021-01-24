using System.Collections;
using Multiverse.Utils;

namespace Multiverse.Tests.Base
{
    public abstract class MultiverseClientFixture : MultiverseTestFixture
    {
        protected override IEnumerator UnityOneTimeSetUp()
        {
            yield return new WaitForTask(NetworkManager.Matchmaker.Connect());
            StartTestServer();
            yield return WaitForTestServer();
            yield return new WaitForTask(JoinServerMatch());
            
            StartTestClient();
            yield return new WaitUntilTimeout(() => NetworkManager.Client.Connections.Count > 2);
        }
    }
}