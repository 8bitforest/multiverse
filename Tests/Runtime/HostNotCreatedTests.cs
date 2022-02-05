using System.Threading.Tasks;
using Multiverse.Tests.Assets.Scripts;
using Multiverse.Tests.Base;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    public abstract class HostNotCreatedTests : MultiverseTestFixture
    {
        [AsyncOneTimeSetUp]
        protected async Task AsyncOneTimeSetUp()
        {
            await NetworkManager.Matchmaker.Connect<TestMatchData>();
        }

        [Test]
        public void NotStartedMatch()
        {
            Assert.False(NetworkManager.IsConnected);
            Assert.False(NetworkManager.IsClient);
            Assert.False(NetworkManager.IsHost);
            Assert.Null(NetworkManager.Server);
            Assert.Null(NetworkManager.Client);
            Assert.Null(NetworkManager.Host);
        }

        [AsyncTest]
        public async Task HostCreatesStopsMatch()
        {
            var onConnectedCalled = NetworkManager.OnConnected.Wait(Multiverse.Timeout);
            await HostServerMatch();
            Assert.IsTrue(NetworkManager.IsConnected);
            Assert.IsTrue(NetworkManager.IsHost);
            Assert.IsFalse(NetworkManager.IsClient);
            Assert.NotNull(NetworkManager.Server);
            Assert.NotNull(NetworkManager.Client);
            Assert.NotNull(NetworkManager.Host);
            Assert.AreEqual(1, NetworkManager.Client.Players.Count);
            await onConnectedCalled;

            var onDisconnectedCalled = NetworkManager.OnDisconnected.Wait(Multiverse.Timeout);
            await NetworkManager.Host.Disconnect();
            Assert.False(NetworkManager.IsConnected);
            Assert.False(NetworkManager.IsClient);
            Assert.False(NetworkManager.IsHost);
            Assert.Null(NetworkManager.Server);
            Assert.Null(NetworkManager.Client);
            Assert.Null(NetworkManager.Host);
            await onDisconnectedCalled;
            
            await NetworkManager.Matchmaker.Connect<TestMatchData>();
        }
    }
}