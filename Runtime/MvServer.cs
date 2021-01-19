using System.Threading.Tasks;
using Reaction;

namespace Multiverse
{
    public class MvServer
    {
        public bool Connected { get; private set; } = true;
        public RxnEvent OnDisconnected { get; }

        private readonly IMvLibraryServer _server;

        public MvServer(IMvLibraryServer server)
        {
            _server = server;
            OnDisconnected = _server.OnDisconnected;
            OnDisconnected.OnInvoked(null, () => Connected = false);
        }

        public async Task Disconnect() => await _server.Disconnect();
    }
}