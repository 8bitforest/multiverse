using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Multiverse.Tests.Backend.Utils
{
    public abstract class MultiverseTestSetUp<T> where T : IMvLibraryTestSuite, new()
    {
        private HashSet<GameObject> _originalObjects = new HashSet<GameObject>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // TODO: Does this run for every test???
            Debug.Log($"Setting up tests for {new T().Name}");
            _originalObjects = new HashSet<GameObject>(Resources.FindObjectsOfTypeAll<GameObject>());
            new GameObject("TestManager").AddComponent<MultiverseTestManager>().SetSuite(new T());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Debug.Log("Cleaning up tests");
            foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
                if (!_originalObjects.Contains(go) && go.hideFlags == HideFlags.None && go.scene.name != null)
                    Object.DestroyImmediate(go);
        }
    }
}