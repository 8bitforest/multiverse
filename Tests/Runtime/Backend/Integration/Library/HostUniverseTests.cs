using Multiverse.Tests.Backend.Base;
using Multiverse.Tests.Backend.Utils;
using NUnit.Framework;
using UnityEngine;

namespace Multiverse.Tests.Backend.Integration.Library
{
    [MultiverseBackendFixture]
    public abstract class HostUniverseTests : MultiverseHostFixture
    {
        [Test]
        public void CantSpawnNotMultiversePrefab()
        {
            Assert.Throws<MvException>(() => NetworkManager.Universe.Instantiate(TestObjects.NotMultiversePrefab));
        }

        [Test]
        public void SpawnsServerOnlyPrefab()
        {
            var instance = NetworkManager.Universe.Instantiate(TestObjects.ServerOnlyPrefab);
            Assert.NotNull(instance);
            Assert.Greater(instance.Id, 0);
            Assert.IsTrue(instance.HasOwnership);
            Assert.AreEqual(NetworkManager.Client.LocalPlayer, instance.Owner);
            Assert.IsTrue(instance.Owner.IsLocal);
            Assert.AreEqual(TestObjects.ServerOnlyPrefab.transform.position, instance.transform.position);
            Assert.AreEqual(TestObjects.ServerOnlyPrefab.transform.rotation, instance.transform.rotation);
        }

        [Test]
        public void SpawnsPrefab()
        {
            var instance = NetworkManager.Universe.Instantiate(TestObjects.ClientServerPrefab);
            Assert.NotNull(instance);
            Assert.Greater(instance.Id, 0);
            Assert.IsTrue(instance.HasOwnership);
            Assert.AreEqual(NetworkManager.Client.LocalPlayer, instance.Owner);
            Assert.IsTrue(instance.Owner.IsLocal);
            Assert.AreEqual(TestObjects.ClientServerPrefab.transform.position, instance.transform.position);
            Assert.AreEqual(TestObjects.ClientServerPrefab.transform.rotation, instance.transform.rotation);
        }

        [Test]
        public void SpawnsPrefabPosition()
        {
            var instance = NetworkManager.Universe.Instantiate(TestObjects.ClientServerPrefab, new Vector3(100, 100));
            Assert.Greater(instance.Id, 0);
            Assert.AreEqual(new Vector3(100, 100, 0), instance.transform.position);
        }

        [Test]
        public void SpawnsPrefabRotation()
        {
            var instance = NetworkManager.Universe.Instantiate(TestObjects.ClientServerPrefab, new Vector3(100, 100),
                Quaternion.Euler(90, 45, 15));
            Assert.Greater(instance.Id, 0);
            Assert.AreEqual(new Vector3(100, 100, 0), instance.transform.position);
            Assert.AreEqual(Quaternion.Euler(90, 45, 15), instance.transform.rotation);
        }
    }
}