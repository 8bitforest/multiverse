using System.Threading.Tasks;
using Multiverse.Tests.Assets.Scripts;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests.Base
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