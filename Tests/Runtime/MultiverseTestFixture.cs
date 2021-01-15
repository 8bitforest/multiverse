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
    public abstract class MultiverseTestFixture
    {
        protected MvNetworkManager NetworkManager { get; private set; }
        protected MultiverseTestManager TestManager { get; private set; }

        private bool _setup;
        private Process _serverProcess;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestManager = Object.FindObjectOfType<MultiverseTestManager>();
            TestManager.DisconnectAndReset();
            NetworkManager = MvNetworkManager.I;
        }

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            if (!_setup)
                yield return UnityOneTimeSetUp();
            _setup = true;
        }

        protected virtual IEnumerator UnityOneTimeSetUp()
        {
            yield break;
        }

        protected void StartTestServer()
        {
            var libraryName = GetType().Namespace.Replace("Tests", "").Trim('.').Split('.').Last();
            var path = Path.Combine(Application.temporaryCachePath, "TestServer" + libraryName);
            if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
                _serverProcess = Process.Start(Path.Combine(path + ".app", "Contents/MacOS/Multiverse"), "-batchmode");
            else
                _serverProcess = Process.Start(path, "-batchmode");
        }

        protected void StopTestServer()
        {
            _serverProcess.Kill();
        }

        protected IEnumerator WaitForTestServer()
        {
            yield return new WaitForTask(NetworkManager.Matchmaker.Connect());
            yield return new WaitUntilTimeout(() =>
                NetworkManager.Matchmaker.Matches.Any(m => m.Name == "Test Server"), 15);
        }

        protected async Task JoinServerMatch()
        {
            var match = await FindServerMatch();
            await NetworkManager.Matchmaker.JoinMatch(match);
        }

        protected async Task<IMvMatch> FindServerMatch()
        {
            var matches = (await NetworkManager.Matchmaker.GetMatchList()).ToArray();
            Assert.IsNotEmpty(matches);
            return matches.FirstOrDefault(m => m.Name == "Test Server");
        }
    }
}