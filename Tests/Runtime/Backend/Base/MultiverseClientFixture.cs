using System.Threading.Tasks;
using Multiverse.Tests.Assets.Scripts;
using UnityEngine.TestTools;

namespace Multiverse.Tests.Backend.Base
{
    public abstract class MultiverseClientFixture : MultiverseTestFixture
    {
        [AsyncOneTimeSetUp]
        public async Task AsyncOneTimeSetUp()
        {
            await NetworkManager.Matchmaker.Connect<TestMatchData>();
            await StartTestServer();
            await JoinServerMatch();
            await StartTestClient();
            // TODO: Wait for other client to join?
        }
    }
}