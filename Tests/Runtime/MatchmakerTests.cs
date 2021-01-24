using System.Collections;
using Multiverse.Tests.Base;
using Multiverse.Tests.Extensions;
using Multiverse.Utils;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    public abstract class MatchmakerTests : MultiverseTestFixture
    {
        protected override IEnumerator UnityOneTimeSetUp()
        {
            yield return new WaitForTask(NetworkManager.Matchmaker.Connect());
        }

        [Test]
        public void MatchmakerNotNull()
        {
            Assert.NotNull(NetworkManager.Matchmaker);
        }

        [Test]
        public void MatchmakerConnected()
        {
            Assert.True(NetworkManager.Matchmaker.Connected);
        }

        [UnityTest]
        public IEnumerator MatchmakerDisconnectsConnects()
        {
                yield return new WaitForTask(async () =>
            {
                await NetworkManager.Matchmaker.Disconnect();
                Assert.False(NetworkManager.Matchmaker.Connected);
                await NetworkManager.Matchmaker.Connect();
                Assert.True(NetworkManager.Matchmaker.Connected);
            });
        }
    }
}