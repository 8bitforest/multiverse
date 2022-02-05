using System.Linq;
using UnityEngine;

namespace Multiverse.Tests.Assets.Scripts
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
            Debug.Log("Test app is running in server mode. Hosting match...");
            await _networkManager.Matchmaker.Connect<TestMatchData>();
            await _networkManager.Matchmaker.HostMatch(MultiverseTestConstants.MatchName, 4, TestMatchData.Create());
            _networkManager.Server.AddMessageReceiver<ServerRequestMessage>(ServerRequest);
            _networkManager.Server.AddMessageReceiver<StopServerMessage>(Application.Quit);
        }

        private void ServerRequest(MvPlayer player, ServerRequestMessage msg)
        {
            switch (msg.RequestType)
            {
                case ServerRequestType.RespondToSender:
                    _networkManager.Server.SendMessageToPlayer(player, new ServerResponseMessage {Id = msg.Id});
                    break;
                case ServerRequestType.RespondToAll:
                    _networkManager.Server.SendMessageToAll(new ServerResponseMessage {Id = msg.Id});
                    break;
                case ServerRequestType.RequestOtherClientToRequestServer:
                    var otherClient = _networkManager.Client.OtherPlayers.First(c => !Equals(c, player));
                    _networkManager.Server.SendMessageToPlayer(otherClient,
                        new ClientRequestMessage {RequestType = ClientRequestType.RequestServer, Id = msg.Id});
                    break;
            }
        }
    }
}