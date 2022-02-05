using System.Threading.Tasks;
using Multiverse.LibraryInterfaces;
using Multiverse.Utils;
using Reaction;

namespace Multiverse
{
    public class MvHost
    {
        public RxnValue<bool> Connected { get; }

        private readonly IMvLibraryHost _host;
        private readonly MvServer _server;
        private readonly MvClient _client;

        public MvHost(IMvLibraryHost host)
        {
            _host = host;
            _server = MvNetworkManager.I.Server;
            _client = MvNetworkManager.I.Client;

            Connected = new RxnValue<bool>();
            _server.Connected.RelayChanged(null, UpdateConnected);
            _client.Connected.RelayChanged(null, UpdateConnected);

            _server.PlayerConnected(new MvPlayer(MvIdManager.AllocatePlayerId(), _host.HostLibId, true, true));
        }

        private void UpdateConnected()
        {
            if (!Connected && _server.Connected && _client.Connected)
                Connected.AsOwner.Set(true);
            else if (Connected && !_server.Connected && !_client.Connected)
                Connected.AsOwner.Set(false);
        }

        public async Task Disconnect()
        {
            await _client.Disconnect();
            await _server.Disconnect();
        }
    }
}