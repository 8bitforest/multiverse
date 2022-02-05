using System.Threading.Tasks;
using Multiverse.Tests.Base;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    // TODO: Make sure existing objects are sent to client
    public abstract class ClientUniverseTests : MultiverseClientFixture
    {
        [Test]
        public void CantSpawnServerOnlyPrefab()
        {
            Assert.Throws<MvException>(() => NetworkManager.Universe.Instantiate(TestObjects.ServerOnlyPrefab));
        }

        [Test]
        public void CantSpawnNotMultiversePrefab()
        {
            Assert.Throws<MvException>(() => NetworkManager.Universe.Instantiate(TestObjects.NotMultiversePrefab));
        }

        [AsyncTest]
        public async Task SpawnsPrefab()
        {
            var instance = NetworkManager.Universe.Instantiate(TestObjects.ClientServerPrefab);
            Assert.NotNull(instance);
            Assert.AreEqual(0, instance.Id);
            Assert.IsTrue(instance.HasOwnership);
            Assert.AreEqual(NetworkManager.Client.LocalPlayer, instance.Owner);
            Assert.IsTrue(instance.Owner.IsLocal);

            await WaitUntil(() => instance.Id > 0);
            Assert.Greater(instance.Id, 0);
            Assert.IsTrue(instance.HasOwnership);
            Assert.AreEqual(NetworkManager.Client.LocalPlayer, instance.Owner);
            Assert.IsTrue(instance.Owner.IsLocal);
            Assert.AreEqual(TestObjects.ClientServerPrefab.transform.position, instance.transform.position);
            Assert.AreEqual(TestObjects.ClientServerPrefab.transform.rotation, instance.transform.rotation);
        }

        [AsyncTest]
        public async Task SpawnsPrefabPosition()
        {
            var instance = NetworkManager.Universe.Instantiate(TestObjects.ClientServerPrefab, new Vector3(100, 100));
            Assert.AreEqual(0, instance.Id);

            await WaitUntil(() => instance.Id > 0);
            Assert.AreEqual(new Vector3(100, 100, 0), instance.transform.position);
        }

        [AsyncTest]
        public async Task SpawnsPrefabRotation()
        {
            var instance = NetworkManager.Universe.Instantiate(TestObjects.ClientServerPrefab, new Vector3(100, 100),
                Quaternion.Euler(90, 45, 15));
            Assert.AreEqual(0, instance.Id);

            await WaitUntil(() => instance.Id > 0);
            Assert.AreEqual(new Vector3(100, 100, 0), instance.transform.position);
            Assert.AreEqual(Quaternion.Euler(90, 45, 15), instance.transform.rotation);
        }
    }
}