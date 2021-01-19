using System.Collections;
using System.Linq;
using Multiverse.Tests.Extensions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    public abstract class ClientJoinedTests : MultiverseTestFixture
    {
        protected override IEnumerator UnityOneTimeSetUp()
        {
            StartTestServer();
            yield return WaitForTestServer();
            yield return new WaitForTask(JoinServerMatch());
        }

        [Test]
        public void JoinedMatch()
        {
            Assert.True(NetworkManager.IsConnected);
            Assert.True(NetworkManager.IsClient);
            Assert.False(NetworkManager.IsHost);
            Assert.Null(NetworkManager.Server);
            Assert.NotNull(NetworkManager.Client);
            Assert.Null(NetworkManager.Host);
            Assert.GreaterOrEqual(NetworkManager.Client.Connections.Count, 1);

            Assert.NotNull(NetworkManager.Client.LocalConnection);
            Assert.False(NetworkManager.Client.LocalConnection.IsHost);
            Assert.True(NetworkManager.Client.LocalConnection.IsLocal);
        }

        [UnityTest]
        public IEnumerator OnDisconnectCalled()
        {
            var onDisconnectedCalled = AssertExtensions.EventCalled(NetworkManager.OnDisconnected);
            StopTestServer();
            yield return onDisconnectedCalled;

            yield return new WaitUntilTimeout(() => NetworkManager.Matchmaker.Matches.Count == 0, 15);

            StartTestServer();
            yield return WaitForTestServer();
            yield return new WaitForTask(JoinServerMatch());
        }

        [UnityTest]
        public IEnumerator OtherClientConnects()
        {
            StartTestClient();
            yield return new WaitUntilTimeout(() => NetworkManager.Client.Connections.Count > 2, 15);
            Assert.AreEqual(2, NetworkManager.Client.OtherConnections.Count());
            var otherClients = NetworkManager.Client.OtherConnections.ToArray();
            var hostClient = otherClients.FirstOrDefault(c => c.IsHost);
            var otherClient = otherClients.FirstOrDefault(c => !Equals(c, hostClient));
            Assert.NotNull(hostClient);
            Assert.False(hostClient.IsLocal);
            Assert.True(hostClient.IsHost);
            Assert.NotNull(otherClient);
            Assert.False(otherClient.IsLocal);
            Assert.False(otherClient.IsHost);
            Assert.AreNotEqual(hostClient.Id, NetworkManager.Client.LocalConnection.Id);
            Assert.AreNotEqual(otherClient.Id, NetworkManager.Client.LocalConnection.Id);

            StopTestClients();
            yield return new WaitUntilTimeout(() => NetworkManager.Client.Connections.Count == 2, 45);
            Assert.AreEqual(1, NetworkManager.Client.OtherConnections.Count());
        }
    }
}