using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Multiverse.Tests.Utils;
using Multiverse.Utils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Multiverse.Tests.Base
{
    public abstract class MultiverseTestFixture
    {
        protected MvNetworkManager NetworkManager { get; private set; }
        protected MultiverseTestManager TestManager { get; private set; }

        private HashSet<Process> _clientProcesses = new HashSet<Process>();
        private Process _serverProcess;

        #region SetUp/TearDown methods

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            TestManager = Object.FindObjectOfType<MultiverseTestManager>();
            TestManager.AddNetworkLibrary();
            NetworkManager = MvNetworkManager.I;
            OneTimeSetUp();
        }

        [UnityOneTimeSetUp]
        public IEnumerator UnityOneTimeSetUpBase()
        {
            yield return UnityOneTimeSetUp();
        }

        [SetUp]
        public void SetUpBase()
        {
            SetUp();
        }

        [UnitySetUp]
        public IEnumerator UnitySetUpBase()
        {
            yield return UnitySetUp();
        }

        [TearDown]
        public void TearDownBase()
        {
            NetworkManager.ClearAllMessageReceivers();
            TearDown();
        }

        [UnityTearDown]
        public IEnumerator UnityTearDownBase()
        {
            yield return UnityTearDown();
        }

        [OneTimeTearDown]
        public void OneTimeTearDownBase()
        {
            StopTestClients();
            StopTestServer();
            OneTimeTearDown();
        }

        [UnityOneTimeTearDown]
        public IEnumerator UnityOneTimeTearDownBase()
        {
            if (NetworkManager.IsConnected && NetworkManager.IsHost)
                yield return new WaitForTask(NetworkManager.Host.Disconnect());
            else if (NetworkManager.IsConnected && NetworkManager.IsClient)
                yield return new WaitForTask(NetworkManager.Client.Disconnect());
            if (NetworkManager.Matchmaker.Connected)
                yield return new WaitForTask(NetworkManager.Matchmaker.Disconnect());
            yield return UnityOneTimeTearDown();
        }

        protected virtual void OneTimeSetUp() { }
        protected virtual IEnumerator UnityOneTimeSetUp() => null;
        protected virtual void SetUp() { }
        protected virtual IEnumerator UnitySetUp() => null;
        protected virtual void TearDown() { }
        protected virtual IEnumerable UnityTearDown() => null;
        protected virtual void OneTimeTearDown() { }
        protected virtual IEnumerator UnityOneTimeTearDown() => null;

        #endregion

        protected void StartTestClient()
        {
            _clientProcesses.Add(StartTestBinary("-client"));
        }

        protected void StartTestServer()
        {
            _serverProcess = StartTestBinary("-server");
        }

        private Process StartTestBinary(string arg)
        {
            var libraryName = GetType().Namespace.Replace("Tests", "").Trim('.').Split('.').Last();
            var path = Path.Combine(Application.temporaryCachePath, $"MultiverseTest{libraryName}");
            if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
                return Process.Start(Path.Combine(path + ".app", "Contents/MacOS/Multiverse"),
                    $"-batchmode {arg}");

            return Process.Start(path, $"-batchmode {arg}");
        }

        protected void StopTestServer()
        {
            _serverProcess?.Kill();
        }

        protected void StopTestClients()
        {
            foreach (var p in _clientProcesses)
                p.Kill();
            _clientProcesses.Clear();
        }

        protected IEnumerator WaitForTestServer()
        {
            yield return new WaitForTask(NetworkManager.Matchmaker.Connect());
            yield return new WaitUntilTimeout(() =>
                NetworkManager.Matchmaker.Matches.Any(m => m.Name == "Test Server"), 15);
        }

        protected async Task HostServerMatch()
        {
            await NetworkManager.Matchmaker.CreateMatch("Test Server", 4);
        }

        protected async Task JoinServerMatch()
        {
            var match = await FindServerMatch();
            await NetworkManager.Matchmaker.JoinMatch(match);
        }

        protected async Task<MvMatch> FindServerMatch()
        {
            var matches = (await NetworkManager.Matchmaker.GetMatchList()).ToArray();
            Assert.IsNotEmpty(matches);
            return matches.FirstOrDefault(m => m.Name == "Test Server");
        }
    }
}