using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Multiverse.Tests.Assets.Scripts
{
    public class TestClient : MonoBehaviour
    {
        private MvNetworkManager _networkManager;
        private MultiverseTestObjects _testObjects;

        private void Start()
        {
            _networkManager = FindObjectOfType<MvNetworkManager>();
            _testObjects = Resources.Load<MultiverseTestObjects>("MultiverseTestObjects");
            JoinServer();
        }

        private async void JoinServer()
        {
            Debug.Log("Test app is running in client mode. Joining match...");
            await _networkManager.Matchmaker.Connect<TestMatchData>();

            MvMatch match = null;
            while (match == null)
            {
                var matches = _networkManager.Matchmaker.Matches.Values.ToArray();
                match = matches.FirstOrDefault(m => m.Name == MultiverseTestConstants.MatchName);
                await Task.Delay(250);
            }

            await _networkManager.Matchmaker.JoinMatch(match);
            _networkManager.Client.AddMessageReceiver<ClientRequestMessage>(ClientRequest);
            _networkManager.Client.AddMessageReceiver<StopClientMessage>(Application.Quit);
        }

        private void ClientRequest(ClientRequestMessage msg)
        {
            switch (msg.RequestType)
            {
                case ClientRequestType.RespondToServer:
                    _networkManager.Client.SendMessageToServer(new ClientResponseMessage {Id = msg.Id});
                    break;
                case ClientRequestType.RequestServer:
                    _networkManager.Client.SendMessageToServer(new ServerRequestMessage
                        {RequestType = ServerRequestType.RespondToAll, Id = msg.Id});
                    break;
            }
        }
    }
}