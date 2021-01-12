using Multiverse.PUN2;
using NUnit.Framework;
using Tests.CommonPlayModeTests;
using UnityEngine;

namespace Tests.Pun2PlaymodeTests
{
    [SetUpFixture]
    public class Pun2TestSetUp : MultiverseTestSetUp<Pun2LibraryAdder> { }
    
    [TestFixture]
    public class Pun2MatchmakerTests : MatchmakerTests { }
    
    [TestFixture]
    public class Pun2MatchmakerClientTests : MatchmakerClientTests { }

    public class Pun2LibraryAdder : IMvTestLibraryAdder
    {
        public void AddLibrary(GameObject gameObject)
        {
            gameObject.AddComponent<Pun2MvLibrary>();
        }
    }
}