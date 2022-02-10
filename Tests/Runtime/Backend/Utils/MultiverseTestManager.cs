using UnityEngine;

namespace Multiverse.Tests.Backend.Utils
{
    public class MultiverseTestManager : MonoBehaviour
    {
        private IMvLibraryTestSuite _suite;

        private GameObject _libraryGameObject;

        public void SetSuite(IMvLibraryTestSuite suite)
        {
            _suite = suite;
        }

        public void AddNetworkLibrary()
        {
            if (_libraryGameObject)
                DestroyImmediate(_libraryGameObject);

            _libraryGameObject = new GameObject("Network Library");
            _suite.AddLibrary(_libraryGameObject);
            var nm = _libraryGameObject.AddComponent<MvNetworkManager>();
            nm.SetTimeout(5);
        }
    }
}