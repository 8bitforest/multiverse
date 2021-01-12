using NUnit.Framework;
using UnityEngine;

namespace Tests.CommonPlayModeTests
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