using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Multiverse.Tests.Scenes.Scripts
{
    public class TestClient : MonoBehaviour
    {
        private MvNetworkManager _networkManager;

        private void Start()
        {
            _networkManager = FindObjectOfType<MvNetworkManager>();
            JoinServer();
        }

        private async void JoinServer()
        {
            await _networkManager.Matchmaker.Connect();

            IMvMatch match = null;
            while (match == null)
            {
                var matches = (await _networkManager.Matchmaker.GetMatchList()).ToArray();
                match = matches.FirstOrDefault(m => m.Name == "Test Server");
                await Task.Delay(1000);
            }

            await _networkManager.Matchmaker.JoinMatch(match);
        }
    }
}