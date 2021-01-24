using System.Collections;
using Multiverse.Tests.Base;
using Multiverse.Tests.Scenes.Scripts;
using Multiverse.Utils;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    public abstract class ClientMessageTests : MultiverseClientFixture
    {
        [UnityTest]
        public IEnumerator SendsAndReceivesMessage()
        {
            var id = -1;
            MvClient.AddMessageReceiver<ServerResponseMessage>(msg => id = msg.Id);
            NetworkManager.Client.SendMessageToServer(new ServerRequestMessage
            {
                RequestType = ServerRequestType.RespondToSender,
                Id = 100
            });

            yield return new WaitUntilTimeout(() => id >= 0);
            Assert.AreEqual(100, id);
        }

        [UnityTest]
        public IEnumerator SendsAndReceivesMessageFromOtherClient()
        {
            var id = -1;
            MvClient.AddMessageReceiver<ServerResponseMessage>(msg => id = msg.Id);
            NetworkManager.Client.SendMessageToServer(new ServerRequestMessage
            {
                RequestType = ServerRequestType.RequestOtherClientToRequestServer,
                Id = 101
            });

            yield return new WaitUntilTimeout(() => id >= 0);
            Assert.AreEqual(101, id);
        }

        [UnityTest]
        public IEnumerator SendsAndReceivesMessageAll()
        {
            var id = -1;
            MvClient.AddMessageReceiver<ServerResponseMessage>(msg => id = msg.Id);
            NetworkManager.Client.SendMessageToServer(new ServerRequestMessage
            {
                RequestType = ServerRequestType.RespondToAll,
                Id = 102
            });

            yield return new WaitUntilTimeout(() => id >= 0);
            Assert.AreEqual(102, id);
        }
    }
}