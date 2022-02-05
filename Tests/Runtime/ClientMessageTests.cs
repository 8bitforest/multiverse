using System.Threading.Tasks;
using Multiverse.Tests.Assets.Scripts;
using Multiverse.Tests.Base;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    public abstract class ClientMessageTests : MultiverseClientFixture
    {
        [AsyncTest]
        public async Task SendsAndReceivesMessage()
        {
            var id = -1;
            NetworkManager.Client.AddMessageReceiver<ServerResponseMessage>(msg => id = msg.Id);
            NetworkManager.Client.SendMessageToServer(new ServerRequestMessage
            {
                RequestType = ServerRequestType.RespondToSender,
                Id = 100
            });

            await WaitUntil(() => id >= 0);
            Assert.AreEqual(100, id);
        }

        [AsyncTest]
        public async Task SendsAndReceivesMessageFromOtherClient()
        {
            var id = -1;
            NetworkManager.Client.AddMessageReceiver<ServerResponseMessage>(msg => id = msg.Id);
            NetworkManager.Client.SendMessageToServer(new ServerRequestMessage
            {
                RequestType = ServerRequestType.RequestOtherClientToRequestServer,
                Id = 101
            });

            await WaitUntil(() => id >= 0);
            Assert.AreEqual(101, id);
        }

        [AsyncTest]
        public async Task SendsAndReceivesMessageAll()
        {
            var id = -1;
            NetworkManager.Client.AddMessageReceiver<ServerResponseMessage>(msg => id = msg.Id);
            NetworkManager.Client.SendMessageToServer(new ServerRequestMessage
            {
                RequestType = ServerRequestType.RespondToAll,
                Id = 102
            });

            await WaitUntil(() => id >= 0);
            Assert.AreEqual(102, id);
        }
    }
}