using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Multiverse.Tests.Base;
using Multiverse.Tests.Scenes.Scripts;
using Multiverse.Utils;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests
{
    public abstract class ServerMessageTests : MultiverseServerFixture
    {
        [UnityTest]
        public IEnumerator SendsAndReceivesMessageOneClient()
        {
            var id = -1;
            MvServer.AddMessageReceiver<ClientResponseMessage>(msg => id = msg.Id);
            NetworkManager.Server.SendMessageToClient(NetworkManager.Client.OtherConnections.First(),
                new ClientRequestMessage
                {
                    RequestType = ClientRequestType.RespondToServer,
                    Id = 200
                });

            yield return new WaitUntilTimeout(() => id >= 0);
            Assert.AreEqual(200, id);
        }

        [UnityTest]
        public IEnumerator SendsAndReceivesMessageAll()
        {
            var ids = new List<int>();
            MvClient.AddMessageReceiver<ClientRequestMessage>(msg
                => NetworkManager.Client.SendMessageToServer(new ClientResponseMessage {Id = msg.Id}));
            MvServer.AddMessageReceiver<ClientResponseMessage>(msg => ids.Add(msg.Id));
            NetworkManager.Server.SendMessageToAll(new ClientRequestMessage
            {
                RequestType = ClientRequestType.RespondToServer,
                Id = 201
            });

            yield return new WaitUntilTimeout(() => ids.Count == 2);
            Assert.AreEqual(201, ids[0]);
            Assert.AreEqual(201, ids[1]);
        }

        [UnityTest]
        public IEnumerator HostClientSendsAndReceivesMessage()
        {
            var id = -1;
            MvServer.AddMessageReceiver<ServerRequestMessage>((c, msg)
                => NetworkManager.Server.SendMessageToClient(c, new ServerResponseMessage {Id = msg.Id}));
            MvClient.AddMessageReceiver<ServerResponseMessage>(msg => id = msg.Id);
            NetworkManager.Client.SendMessageToServer(new ServerRequestMessage
            {
                RequestType = ServerRequestType.RespondToSender,
                Id = 202
            });

            yield return new WaitUntilTimeout(() => id >= 0);
            Assert.AreEqual(202, id);
        }
    }
}