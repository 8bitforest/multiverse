using kcp2k;
using Multiverse.MirrorNoble;
using NUnit.Framework;
using Tests.CommonPlayModeTests;
using UnityEngine;

namespace Tests.MirrorNoblePlaymodeTests
{
    [SetUpFixture]
    public class MirrorNobleTestSetUp : MultiverseTestSetUp<MirrorNobleLibraryAdder> { }
    
    [TestFixture]
    public class MirrorNobleMatchmakerTests : MatchmakerTests { }
    
    [TestFixture]
    public class MirrorNobleMatchmakerClientTests : MatchmakerClientTests { }

    public class MirrorNobleLibraryAdder : IMvTestLibraryAdder
    {
        public void AddLibrary(GameObject gameObject)
        {
            gameObject.AddComponent<KcpTransport>();
            gameObject.AddComponent<MirrorNobleMvLibrary>();
        }
    }
}