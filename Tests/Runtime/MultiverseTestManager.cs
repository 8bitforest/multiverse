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

        public void DisconnectAndReset()
        {
            if (_libraryGameObject)
                DestroyImmediate(_libraryGameObject);

            _libraryGameObject = new GameObject("Network Library");
            _adder.AddLibrary(_libraryGameObject);
            _libraryGameObject.AddComponent<MvNetworkManager>();
        }
        
        public void LeaveMatchAndReset()
        {
            // TODO
        }
    }

    public interface IMvTestLibraryAdder
    {
        void AddLibrary(GameObject gameObject);
    }
}