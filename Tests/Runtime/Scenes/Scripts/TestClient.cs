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

            MvMatch match = null;
            while (match == null)
            {
                var matches = (await _networkManager.Matchmaker.GetMatchList()).ToArray();
                match = matches.FirstOrDefault(m => m.Name == "Test Server");
                await Task.Delay(1000);
            }

            MvClient.AddMessageReceiver<ClientRequestMessage>(ClientRequest);

            await _networkManager.Matchmaker.JoinMatch(match);
        }

        private void ClientRequest(MvConnection connection, ClientRequestMessage msg)
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