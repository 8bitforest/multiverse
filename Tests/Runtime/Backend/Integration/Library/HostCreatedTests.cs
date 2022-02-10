using System.Linq;
using System.Threading.Tasks;
using Multiverse.Tests.Assets.Scripts;
using Multiverse.Tests.Backend.Base;
using Multiverse.Tests.Backend.Utils;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests.Backend.Integration.Library
{
    [MultiverseBackendFixture]
    public abstract class HostCreatedTests : MultiverseTestFixture
    {
        [AsyncOneTimeSetUp]
        protected async Task UnityOneTimeSetUp()
        {
            await NetworkManager.Matchmaker.Connect<TestMatchData>();
            await HostServerMatch();
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
            Assert.AreEqual(1, NetworkManager.Client.Players.Count);
            Assert.AreEqual(0, NetworkManager.Client.OtherPlayers.Count());
            Assert.AreEqual(1, NetworkManager.Server.Players.Count);

            Assert.NotNull(NetworkManager.Client.LocalPlayer);
            Assert.True(NetworkManager.Client.LocalPlayer.IsHost);
            Assert.True(NetworkManager.Client.LocalPlayer.IsLocal);
        }

        [AsyncTest]
        public async Task ClientConnects()
        {
            // TODO: Check if LocalPlayer is in Players somewhere?
            await StartTestClient();
            Assert.AreEqual(1, NetworkManager.Client.OtherPlayers.Count());
            Assert.AreEqual(2, NetworkManager.Server.Players.Count);
            var otherClient = NetworkManager.Client.OtherPlayers.First();
            Assert.False(otherClient.IsHost);
            Assert.False(otherClient.IsLocal);
            Assert.AreNotEqual(otherClient.Id, NetworkManager.Client.LocalPlayer.Id);

            await KillTestClients();
            Assert.AreEqual(0, NetworkManager.Client.OtherPlayers.Count());
            Assert.AreEqual(1, NetworkManager.Server.Players.Count);
        }
    }
}