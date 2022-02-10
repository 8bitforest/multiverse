using System.Threading.Tasks;
using Multiverse.Tests.Assets.Scripts;
using Multiverse.Tests.Backend.Base;
using Multiverse.Tests.Backend.Utils;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests.Backend.Integration.Library
{
    [MultiverseBackendFixture]
    public abstract class ClientNotJoinedTests : MultiverseTestFixture
    {
        [AsyncOneTimeSetUp]
        public async Task AsyncOneTimeSetUp()
        {
            await StartTestServer();
            await WaitForTestServer();
        }

        [Test]
        public void NotJoinedMatch()
        {
            Assert.False(NetworkManager.IsConnected);
            Assert.False(NetworkManager.IsClient);
            Assert.False(NetworkManager.IsHost);
            Assert.Null(NetworkManager.Server);
            Assert.Null(NetworkManager.Client);
            Assert.Null(NetworkManager.Host);
        }

        [Test]
        public void MatchExists()
        {
            var match = FindServerMatch();
            Assert.IsNotNull(match);
        }

        [Test]
        public void MatchHas4MaxPlayers()
        {
            var match = FindServerMatch();
            Assert.AreEqual(4, match.MaxPlayers);
        }

        [Test]
        public void MatchHasCustomData()
        {
            var match = FindServerMatch();
            var customData = match.GetCustomData<TestMatchData>();
            var expectedData = TestMatchData.Create();
            Assert.AreEqual(expectedData.StringData, customData.StringData);
            Assert.AreEqual(expectedData.IntData, customData.IntData);
            Assert.AreEqual(expectedData.FloatData, customData.FloatData);
        }

        [AsyncTest]
        public async Task JoinsLeavesMatch()
        {
            var onConnectedCalled = NetworkManager.OnConnected.Wait(global::Multiverse.Multiverse.Timeout);
            await JoinServerMatch();
            Assert.True(NetworkManager.IsConnected);
            Assert.True(NetworkManager.IsClient);
            Assert.False(NetworkManager.IsHost);
            Assert.Null(NetworkManager.Server);
            Assert.NotNull(NetworkManager.Client);
            Assert.Null(NetworkManager.Host);
            Assert.AreEqual(2, NetworkManager.Client.Players.Count);
            await onConnectedCalled;

            var onDisconnectedCalled = NetworkManager.OnDisconnected.Wait(global::Multiverse.Multiverse.Timeout);
            await NetworkManager.Client.Disconnect();
            Assert.False(NetworkManager.IsConnected);
            Assert.False(NetworkManager.IsClient);
            Assert.False(NetworkManager.IsHost);
            Assert.Null(NetworkManager.Server);
            Assert.Null(NetworkManager.Client);
            Assert.Null(NetworkManager.Host);
            await onDisconnectedCalled;

            await WaitForTestServer();
        }
    }
}