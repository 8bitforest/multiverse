using System.Threading.Tasks;
using Reaction;

namespace Multiverse
{
    public class MvHost
    {
        public bool Connected => _server.Connected || _client.Connected;
        public RxnEvent OnDisconnected { get; }
        
        private readonly MvServer _server;
        private readonly MvClient _client;
        
        public MvHost(MvServer server, MvClient client)
        {
            _server = server;
            _client = client;

            OnDisconnected = _server.OnDisconnected;
        }

        public async Task Disconnect()
        {
            await _client.Disconnect();
            await _server.Disconnect();
        }
    }
}