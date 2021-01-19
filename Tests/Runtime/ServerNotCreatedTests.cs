using System.Collections;
using Multiverse.Tests.Extensions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    public abstract class ServerNotCreatedTests : MultiverseTestFixture
    {
        protected override IEnumerator UnityOneTimeSetUp()
        {
            yield return new WaitForTask(NetworkManager.Matchmaker.Connect());
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

        [UnityTest]
        public IEnumerator HostCreatesStopsMatch()
        {
            var onConnectedCalled = AssertExtensions.EventCalled(NetworkManager.OnConnected);
            yield return new WaitForTask(async () =>
            {
                await HostServerMatch();
                Assert.IsTrue(NetworkManager.IsConnected);
                Assert.IsTrue(NetworkManager.IsHost);
                Assert.IsFalse(NetworkManager.IsClient);
                Assert.NotNull(NetworkManager.Server);
                Assert.NotNull(NetworkManager.Client);
                Assert.NotNull(NetworkManager.Host);
                Assert.AreEqual(1, NetworkManager.Client.Connections.Count);
            });
            yield return onConnectedCalled;

            var onDisconnectedCalled = AssertExtensions.EventCalled(NetworkManager.OnDisconnected);
            yield return new WaitForTask(async () =>
            {
                await NetworkManager.Host.Disconnect();
                Assert.False(NetworkManager.IsConnected);
                Assert.False(NetworkManager.IsClient);
                Assert.False(NetworkManager.IsHost);
                Assert.Null(NetworkManager.Server);
                Assert.Null(NetworkManager.Client);
                Assert.Null(NetworkManager.Host);
            });
            yield return onDisconnectedCalled;
            yield return new WaitForTask(NetworkManager.Matchmaker.Connect);
        }
    }
}