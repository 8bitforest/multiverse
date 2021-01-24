using System.Collections;
using Multiverse.Tests.Base;
using Multiverse.Tests.Extensions;
using Multiverse.Utils;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    public abstract class ClientNotJoinedTests : MultiverseTestFixture
    {
        protected override IEnumerator UnityOneTimeSetUp()
        {
            StartTestServer();
            yield return WaitForTestServer();
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

        [UnityTest]
        public IEnumerator MatchExists()
        {
            yield return new WaitForTask(async () =>
            {
                var match = await FindServerMatch();
                Assert.IsNotNull(match);
            });
        }

        [UnityTest]
        public IEnumerator MatchHas4MaxPlayers()
        {
            yield return new WaitForTask(async () =>
            {
                var match = await FindServerMatch();
                Assert.AreEqual(4, match.MaxPlayers);
            });
        }

        [UnityTest]
        public IEnumerator JoinsLeavesMatch()
        {
            var onConnectedCalled = AssertExtensions.EventCalled(NetworkManager.OnConnected);
            yield return new WaitForTask(async () =>
            {
                await JoinServerMatch();
                Assert.True(NetworkManager.IsConnected);
                Assert.True(NetworkManager.IsClient);
                Assert.False(NetworkManager.IsHost);
                Assert.Null(NetworkManager.Server);
                Assert.NotNull(NetworkManager.Client);
                Assert.Null(NetworkManager.Host);
            });
            yield return new WaitUntilTimeout(() => NetworkManager.Client.Connections.Count == 2);
            yield return onConnectedCalled;

            var onDisconnectedCalled = AssertExtensions.EventCalled(NetworkManager.OnDisconnected);
            yield return new WaitForTask(async () =>
            {
                await NetworkManager.Client.Disconnect();
                Assert.False(NetworkManager.IsConnected);
                Assert.False(NetworkManager.IsClient);
                Assert.False(NetworkManager.IsHost);
                Assert.Null(NetworkManager.Server);
                Assert.Null(NetworkManager.Client);
                Assert.Null(NetworkManager.Host);
            });
            yield return onDisconnectedCalled;
            yield return WaitForTestServer();
        }

        [UnityTest]
        public IEnumerator JoinsInvalidMatchThrows()
        {
            LogAssert.ignoreFailingMessages = true;
            yield return new WaitForTask(async () =>
            {
                var match = new MvMatch(null, "0", 4);
                await AssertExtensions.ThrowsAsync<MvException>(NetworkManager.Matchmaker.JoinMatch(match));
            });
        }
    }
}