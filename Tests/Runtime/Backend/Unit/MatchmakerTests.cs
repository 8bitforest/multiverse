using System.Threading.Tasks;
using Multiverse.Tests.Assets.Scripts;
using Multiverse.Tests.Backend.Base;
using Multiverse.Tests.Backend.Utils;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests.Backend.Unit
{
    [MultiverseBackendFixture]
    public abstract class MatchmakerTests : MultiverseTestFixture
    {
        [AsyncOneTimeSetUp]
        protected async Task AsyncOneTimeSetUp()
        {
            await NetworkManager.Matchmaker.Connect<TestMatchData>();
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

        [AsyncTest]
        public async Task MatchmakerDisconnectsConnects()
        {
            await NetworkManager.Matchmaker.Disconnect();
            Assert.False(NetworkManager.Matchmaker.Connected);
            await NetworkManager.Matchmaker.Connect<TestMatchData>();
            Assert.True(NetworkManager.Matchmaker.Connected);
        }
    }
}