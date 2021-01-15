using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Multiverse.Tests.Extensions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    public abstract class MatchmakerClientTests : MultiverseTestFixture
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            StartTestServer();
        }

        protected override IEnumerator UnityOneTimeSetUp()
        {
            yield return WaitForTestServer();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            StopTestServer();
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
            });
            yield return onConnectedCalled;

            var onDisconnectedCalled = AssertExtensions.EventCalled(NetworkManager.OnDisconnected);
            yield return new WaitForTask(async () =>
            {
                await NetworkManager.Client.Disconnect();
                Assert.False(NetworkManager.IsConnected);
                Assert.False(NetworkManager.IsClient);
                Assert.False(NetworkManager.IsHost);
            });
            yield return onDisconnectedCalled;
            yield return WaitForTestServer();
        }

        [UnityTest]
        public IEnumerator OnDisconnectCalled()
        {
            var onDisconnectedCalled = AssertExtensions.EventCalled(NetworkManager.OnDisconnected);
            yield return new WaitForTask(async () =>
            {
                await JoinServerMatch();
                StopTestServer();
            });
            yield return onDisconnectedCalled;
            
            StartTestServer();
            yield return WaitForTestServer();
        }

        [UnityTest]
        public IEnumerator JoinsInvalidMatchThrows()
        {
            LogAssert.ignoreFailingMessages = true;
            yield return new WaitForTask(async () =>
            {
                var match = new DefaultMvMatch {Id = "0"};
                await AssertExtensions.ThrowsAsync<MvException>(NetworkManager.Matchmaker.JoinMatch(match));
            });
        }
    }
}