using kcp2k;
using Multiverse.MirrorNoble;
using NUnit.Framework;
using Tests.CommonPlayModeTests;
using UnityEngine;

namespace Tests.MirrorNoblePlaymodeTests
{
    [TestFixture]
    public class MirrorNobleMatchmakerTests : MatchmakerTests
    {
        protected override void AddLibrary(GameObject gameObject)
        {
            gameObject.AddComponent<KcpTransport>();
            gameObject.AddComponent<MirrorNobleMvNetworkLibrary>();
        }
    }
}