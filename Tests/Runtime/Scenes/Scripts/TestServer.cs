using System.Linq;
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
            MvServer.AddMessageReceiver<ServerRequestMessage>(ServerRequest);

            await _networkManager.Matchmaker.Connect();
            await _networkManager.Matchmaker.CreateMatch("Test Server", 4);
        }

        private void ServerRequest(MvConnection connection, ServerRequestMessage msg)
        {
            switch (msg.RequestType)
            {
                case ServerRequestType.RespondToSender:
                    _networkManager.Server.SendMessageToClient(connection, new ServerResponseMessage {Id = msg.Id});
                    break;
                case ServerRequestType.RespondToAll:
                    _networkManager.Server.SendMessageToAll(new ServerResponseMessage {Id = msg.Id});
                    break;
                case ServerRequestType.RequestOtherClientToRequestServer:
                    var otherClient = _networkManager.Client.OtherConnections.First(c => !Equals(c, connection));
                    _networkManager.Server.SendMessageToClient(otherClient,
                        new ClientRequestMessage {RequestType = ClientRequestType.RequestServer, Id = msg.Id});
                    break;
            }
        }
    }
}