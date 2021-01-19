using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reaction;

namespace Multiverse
{
    public class MvClient
    {
        public bool Connected { get; private set; } = true;
        public IMvConnection LocalConnection => _client.LocalConnection;

        public RxnEvent OnDisconnected { get; }
        public RxnSet<IMvConnection> Connections { get; }
        public IEnumerable<IMvConnection> OtherConnections => Connections.Where(c => !Equals(c, LocalConnection));

        private readonly IMvLibraryClient _client;

        public MvClient(IMvLibraryClient client)
        {
            _client = client;
            OnDisconnected = _client.OnDisconnected;
            Connections = _client.Connections;
            OnDisconnected.OnInvoked(null, () => Connected = false);
        }

        public async Task Disconnect() => await _client.Disconnect();
    }
}