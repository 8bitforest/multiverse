using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Multiverse.Tests.Extensions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    public abstract class MatchmakerClientTests : MultiverseTestFixture
    {
        private Process _serverProcess;

        [OneTimeSetUp]
        public void SetUp()
        {
            var libraryName = GetType().Namespace.Replace("Tests", "").Trim('.').Split('.').Last();
            var path = Path.Combine(Application.temporaryCachePath, "TestServer" + libraryName);
            if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
                _serverProcess = Process.Start(Path.Combine(path + ".app", "Contents/MacOS/Multiverse"), "-batchmode");
            else
                _serverProcess = Process.Start(path, "-batchmode");
        }

        protected override IEnumerator UnityOneTimeSetUp()
        {
            yield return new WaitForTask(NetworkManager.Matchmaker.Connect());
            yield return new WaitUntilTimeout(() =>
                NetworkManager.Matchmaker.Matches.Any(m => m.Name == "Test Server"), 15);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _serverProcess.Kill();
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
                var match = await FindServerMatch();
                await NetworkManager.Matchmaker.JoinMatch(match);
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

        private async Task<IMvMatch> FindServerMatch()
        {
            var matches = (await NetworkManager.Matchmaker.GetMatchList()).ToArray();
            Assert.IsNotEmpty(matches);
            return matches.FirstOrDefault(m => m.Name == "Test Server");
        }
    }
}