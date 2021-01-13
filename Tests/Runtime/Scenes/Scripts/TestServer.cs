using UnityEngine;

namespace Multiverse.Tests.Scenes.Scripts
{
    public class TestServer : MonoBehaviour
    {
        private MvNetworkManager _networkManager;

        private void Start()
        {
            _networkManager = FindObjectOfType<MvNetworkManager>();
            StartServer();
        }

        private async void StartServer()
        {
            await _networkManager.Matchmaker.Connect();
            await _networkManager.Matchmaker.CreateMatch("Test Server", 4);
        }
    }
}