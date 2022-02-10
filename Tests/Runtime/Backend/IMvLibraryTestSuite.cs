using Multiverse.LibraryInterfaces;
using UnityEngine;

namespace Multiverse.Tests.Backend
{
    public interface IMvLibraryTestSuite
    {
        public string Name { get; }
        public IMvLibrary AddLibrary(GameObject gameObject);
    }
}