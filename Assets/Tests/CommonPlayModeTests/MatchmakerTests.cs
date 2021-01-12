using System.Collections;
using System.Collections.Generic;
using Multiverse.Common;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests.CommonPlayModeTests
{
    public abstract class MatchmakerTests
    {
        private HashSet<GameObject> _objects;
        private MvNetworkManager _networkManager;

        [SetUp]
        public void SetUp()
        {
            _objects = new HashSet<GameObject>();

            var go = new GameObject("Test Network Manager");
            _objects.Add(go);

            AddLibrary(go);
            _networkManager = go.AddComponent<MvNetworkManager>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var o in _objects)
                Object.DestroyImmediate(o);
        }

        protected abstract void AddLibrary(GameObject gameObject);

        [Test]
        public void MatchmakerNotNull()
        {
            Assert.NotNull(_networkManager.Matchmaker);
        }
        
        [UnityTest]
        public IEnumerator MatchmakerConnects()
        {
            yield return new WaitForTask(async () =>
            {
                await _networkManager.Matchmaker.Connect();
                Assert.True(_networkManager.Matchmaker.Connected);
            });
        }
        
        [UnityTest]
        public IEnumerator MatchmakerDisconnects()
        {
            yield return new WaitForTask(async () =>
            {
                await _networkManager.Matchmaker.Connect();
                Assert.True(_networkManager.Matchmaker.Connected);
                await _networkManager.Matchmaker.Disconnect();
                Assert.False(_networkManager.Matchmaker.Connected);
            });
        }
    }
}