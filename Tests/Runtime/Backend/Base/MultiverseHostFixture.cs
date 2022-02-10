using System.Threading.Tasks;
using Multiverse.Tests.Assets.Scripts;
using UnityEngine.TestTools;

namespace Multiverse.Tests.Backend.Base
{
    public abstract class MultiverseHostFixture : MultiverseTestFixture
    {
        [AsyncOneTimeSetUp]
        public async Task AsyncOneTimeSetUp()
        {
            await NetworkManager.Matchmaker.Connect<TestMatchData>();
            await HostServerMatch();
            await StartTestClient();
        }
    }
}