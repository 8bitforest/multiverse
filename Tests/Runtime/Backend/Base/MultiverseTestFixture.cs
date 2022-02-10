using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Multiverse.Tests.Assets.Scripts;
using Multiverse.Tests.Backend.Utils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Multiverse.Tests.Backend.Base
{
    public abstract class MultiverseTestFixture
    {
        protected MvNetworkManager NetworkManager { get; private set; }
        protected MultiverseTestManager TestManager { get; private set; }
        protected MultiverseTestObjects TestObjects { get; private set; }

        private readonly HashSet<Process> _clientProcesses = new HashSet<Process>();
        private Process _serverProcess;

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            TestManager = Object.FindObjectOfType<MultiverseTestManager>();
            TestManager.AddNetworkLibrary();
            NetworkManager = MvNetworkManager.I;
            TestObjects = Resources.Load<MultiverseTestObjects>("MultiverseTestObjects");
        }

        [AsyncOneTimeTearDown]
        public async Task AsyncOneTimeTearDownBase()
        {
            if (NetworkManager.IsConnected && NetworkManager.IsHost)
                await NetworkManager.Host.Disconnect();
            else if (NetworkManager.IsConnected && NetworkManager.IsClient)
                await NetworkManager.Client.Disconnect();

            if (NetworkManager.Matchmaker.Connected)
                await NetworkManager.Matchmaker.Disconnect();

            await KillTestClients();
            await KillTestServer();
        }

        [TearDown]
        public void TearDown()
        {
            if (NetworkManager.HasClient)
                NetworkManager.Client.ClearMessageReceivers();
            if (NetworkManager.HasServer)
                NetworkManager.Server.ClearMessageReceivers();
        }

        protected async Task StartTestClient()
        {
            var currentClients = NetworkManager.Client.Players.Count;
            _clientProcesses.Add(StartTestBinary("-client"));
            await WaitUntil(() => NetworkManager.Client.Players.Count == currentClients + 1);
        }

        protected async Task StartTestServer()
        {
            _serverProcess = StartTestBinary("-server");
            await WaitForTestServer();
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

        protected async Task KillTestServer()
        {
            _serverProcess?.Kill();
            await WaitUntil(() => !NetworkManager.Connected, 60);
        }

        protected async Task KillTestClients()
        {
            if (_clientProcesses == null)
                return;

            foreach (var p in _clientProcesses)
                p.Kill();

            if (NetworkManager.HasClient)
            {
                var targetPlayers = NetworkManager.Client.Players.Count - _clientProcesses.Count;
                await WaitUntil(() => NetworkManager.Client.Players.Count <= targetPlayers, 60);
            }

            _clientProcesses.Clear();
        }

        protected async Task WaitForTestServer()
        {
            await NetworkManager.Matchmaker.Connect<TestMatchData>();
            await WaitUntil(() => FindServerMatch() != null);
        }

        protected async Task HostServerMatch()
        {
            await NetworkManager.Matchmaker.HostMatch(MultiverseTestConstants.MatchName, 4, TestMatchData.Create());
        }

        protected async Task JoinServerMatch()
        {
            var match = FindServerMatch();
            await NetworkManager.Matchmaker.JoinMatch(match);
        }

        protected MvMatch FindServerMatch()
        {
            var matches = NetworkManager.Matchmaker.Matches.Values.ToArray();
            return matches.FirstOrDefault(m => m.Name == MultiverseTestConstants.MatchName);
        }

        protected async Task WaitUntil(Func<bool> predicate, float timeout = global::Multiverse.Multiverse.Timeout)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (!predicate())
            {
                if (stopwatch.ElapsedMilliseconds > timeout * 1000)
                    Assert.Fail("WaitUntil timed out");

                await Task.Delay(50);
            }
        }
    }
}