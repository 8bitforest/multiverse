using Multiverse.PUN2;
using NUnit.Framework;
using Tests.CommonPlayModeTests;
using UnityEngine;

namespace Tests.Pun2PlaymodeTests
{
    [TestFixture]
    public class Pun2MatchmakerTests : MatchmakerTests
    {
        protected override void AddLibrary(GameObject gameObject)
        {
            gameObject.AddComponent<Pun2MvNetworkLibrary>();
        }
    }
}