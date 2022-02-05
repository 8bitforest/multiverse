using System.Linq;
using System.Threading.Tasks;
using Multiverse.Tests.Base;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    public abstract class ClientJoinedTests : MultiverseTestFixture
    {
        [AsyncOneTimeSetUp]
        public async Task UnityOneTimeSetUp()
        {
            await StartTestServer();
            await JoinServerMatch();
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
            Assert.AreEqual(2, NetworkManager.Client.Players.Count);

            Assert.NotNull(NetworkManager.Client.LocalPlayer);
            Assert.False(NetworkManager.Client.LocalPlayer.IsHost);
            Assert.True(NetworkManager.Client.LocalPlayer.IsLocal);
        }

        [AsyncTest]
        public async Task OnDisconnectCalled()
        {
            var onDisconnectedCalled = NetworkManager.OnDisconnected.Wait(60);//)Multiverse.Timeout);
            await KillTestServer();
            await onDisconnectedCalled;

            Assert.False(NetworkManager.IsConnected);
            Assert.AreEqual(0, NetworkManager.Matchmaker.Matches.Count);

            await StartTestServer();
            await WaitForTestServer();
            await JoinServerMatch();
        }

        [AsyncTest]
        public async Task OtherClientConnects()
        {
            await StartTestClient();
            Assert.AreEqual(3, NetworkManager.Client.Players.Count);
            Assert.AreEqual(2, NetworkManager.Client.OtherPlayers.Count());
            var otherClients = NetworkManager.Client.OtherPlayers.ToArray();
            var hostClient = otherClients.FirstOrDefault(c => c.IsHost);
            var otherClient = otherClients.FirstOrDefault(c => !Equals(c, hostClient));
            Assert.NotNull(hostClient);
            Assert.False(hostClient.IsLocal);
            Assert.True(hostClient.IsHost);
            Assert.NotNull(otherClient);
            Assert.False(otherClient.IsLocal);
            Assert.False(otherClient.IsHost);
            Assert.AreNotEqual(hostClient.Id, NetworkManager.Client.LocalPlayer.Id);
            Assert.AreNotEqual(otherClient.Id, NetworkManager.Client.LocalPlayer.Id);

            await KillTestClients();
            Assert.AreEqual(2, NetworkManager.Client.Players.Count);
            Assert.AreEqual(1, NetworkManager.Client.OtherPlayers.Count());
        }
    }
}