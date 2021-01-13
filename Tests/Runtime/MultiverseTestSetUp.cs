using NUnit.Framework;
using UnityEngine;

namespace Multiverse.Tests
{
    public abstract class MultiverseTestSetUp<T> where T : IMvTestLibraryAdder, new()
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Debug.Log("Setting up tests");
            new GameObject("Test Manager").AddComponent<MultiverseTestManager>().SetLibraryAdder(new T());
        }
    }
}