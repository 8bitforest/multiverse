using System.Collections;
using System.Linq;
using Multiverse.Tests.Extensions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    public abstract class ServerCreatedTests : MultiverseTestFixture
    {
        protected override IEnumerator UnityOneTimeSetUp()
        {
            yield return new WaitForTask(NetworkManager.Matchmaker.Connect());
            yield return new WaitForTask(HostServerMatch());
        }

        [Test]
        public void StartedMatch()
        {
            Assert.True(NetworkManager.IsConnected);
            Assert.False(NetworkManager.IsClient);
            Assert.True(NetworkManager.IsHost);
            Assert.NotNull(NetworkManager.Server);
            Assert.NotNull(NetworkManager.Client);
            Assert.NotNull(NetworkManager.Host);
            Assert.AreEqual(1, NetworkManager.Client.Connections.Count);
            Assert.AreEqual(0, NetworkManager.Client.OtherConnections.Count());
            
            Assert.NotNull(NetworkManager.Client.LocalConnection);
            Assert.True(NetworkManager.Client.LocalConnection.IsHost);
            Assert.True(NetworkManager.Client.LocalConnection.IsLocal);
        }

        [UnityTest]
        public IEnumerator ClientConnects()
        {
            StartTestClient();
            yield return new WaitUntilTimeout(() => NetworkManager.Client.Connections.Count > 1, 15);
            Assert.AreEqual(1, NetworkManager.Client.OtherConnections.Count());
            var otherClient = NetworkManager.Client.OtherConnections.First();
            Assert.False(otherClient.IsHost);
            Assert.False(otherClient.IsLocal);
            Assert.AreNotEqual(otherClient.Id, NetworkManager.Client.LocalConnection.Id);
            
            StopTestClients();
            yield return new WaitUntilTimeout(() => NetworkManager.Client.Connections.Count == 1, 45);
            Assert.AreEqual(0, NetworkManager.Client.OtherConnections.Count());
        }
    }
}