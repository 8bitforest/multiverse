using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Multiverse.Tests.Assets.Scripts;
using Multiverse.Tests.Backend.Base;
using Multiverse.Tests.Backend.Utils;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Multiverse.Tests.Backend.Integration.Library
{
    [MultiverseBackendFixture]
    public abstract class HostMessageTests : MultiverseHostFixture
    {
        [AsyncTest]
        public async Task SendsAndReceivesMessageOneClient()
        {
            var id = -1;
            NetworkManager.Server.AddMessageReceiver<ClientResponseMessage>(msg => id = msg.Id);
            NetworkManager.Server.SendMessageToPlayer(NetworkManager.Client.OtherPlayers.First(),
                new ClientRequestMessage
                {
                    RequestType = ClientRequestType.RespondToServer,
                    Id = 200
                });

            await WaitUntil(() => id >= 0);
            Assert.AreEqual(200, id);
        }

        [AsyncTest]
        public async Task SendsAndReceivesMessageAll()
        {
            // TODO: Test unreliable
            // TODO: Test ordering
            var ids = new List<int>();
            NetworkManager.Client.AddMessageReceiver<ClientRequestMessage>(msg
                => NetworkManager.Client.SendMessageToServer(new ClientResponseMessage {Id = msg.Id}));
            NetworkManager.Server.AddMessageReceiver<ClientResponseMessage>(msg => ids.Add(msg.Id));
            NetworkManager.Server.SendMessageToAll(new ClientRequestMessage
            {
                RequestType = ClientRequestType.RespondToServer,
                Id = 201
            });

            await WaitUntil(() => ids.Count == 2);
            Assert.AreEqual(201, ids[0]);
            Assert.AreEqual(201, ids[1]);
        }

        [AsyncTest]
        public async Task HostClientSendsAndReceivesMessage()
        {
            var id = -1;
            NetworkManager.Server.AddMessageReceiver<ServerRequestMessage>((c, msg)
                => NetworkManager.Server.SendMessageToPlayer(c, new ServerResponseMessage {Id = msg.Id}));
            NetworkManager.Client.AddMessageReceiver<ServerResponseMessage>(msg => id = msg.Id);
            NetworkManager.Client.SendMessageToServer(new ServerRequestMessage
            {
                RequestType = ServerRequestType.RespondToSender,
                Id = 202
            });

            await WaitUntil(() => id >= 0);
            Assert.AreEqual(202, id);
        }
    }
}