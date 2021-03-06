using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Multiverse.Tests.Utils
{
    public abstract class MultiverseTestSetUp<T> where T : IMvTestLibraryAdder, new()
    {
        private HashSet<GameObject> _originalObjects = new HashSet<GameObject>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Debug.Log("Setting up tests");
            _originalObjects = new HashSet<GameObject>(Resources.FindObjectsOfTypeAll<GameObject>());
            new GameObject("TestManager").AddComponent<MultiverseTestManager>().SetLibraryAdder(new T());
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