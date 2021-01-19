using UnityEngine;

namespace Multiverse.Tests
{
    public class MultiverseTestManager : MonoBehaviour
    {
        private IMvTestLibraryAdder _adder;

        private GameObject _libraryGameObject;

        public void SetLibraryAdder(IMvTestLibraryAdder adder)
        {
            _adder = adder;
        }

        public void AddNetworkLibrary()
        {
            if (_libraryGameObject)
                DestroyImmediate(_libraryGameObject);

            _libraryGameObject = new GameObject("Network Library");
            _adder.AddLibrary(_libraryGameObject);
            var nm = _libraryGameObject.AddComponent<MvNetworkManager>();
            nm.SetTimeout(5);
        }
    }

    public interface IMvTestLibraryAdder
    {
        void AddLibrary(GameObject gameObject);
    }
}