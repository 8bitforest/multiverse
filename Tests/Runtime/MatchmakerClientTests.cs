using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            if (Application.platform == RuntimePlatform.OSXPlayer)
                _serverProcess = Process.Start(Path.Combine(path + ".app", "Contents/MacOS/Multiverse"), "");
            else
                _serverProcess = Process.Start(path);
        }

        protected override IEnumerator UnityOneTimeSetUp()
        {
            yield return new WaitForTask(NetworkManager.Matchmaker.Connect());
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
        public IEnumerator JoinsMatch()
        {
            yield return new WaitForTask(async () =>
            {
                var match = await FindServerMatch();
                await NetworkManager.Matchmaker.JoinMatch(match);
                // TODO: Make sure we are connected to a server
            });
        }

        [UnityTest]
        public IEnumerator JoinsInvalidMatchThrows()
        {
            yield return new WaitForTask(async () =>
            {
                var match = new DefaultMvMatch {Id = "MatchDoesntExist"};
                await NetworkManager.Matchmaker.JoinMatch(match);
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