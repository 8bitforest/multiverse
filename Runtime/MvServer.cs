using System.Threading.Tasks;
using Reaction;

namespace Multiverse
{
    public class MvServer
    {
        public RxnEvent OnDisconnected { get; }

        private readonly IMvLibraryServer _server;

        public MvServer(IMvLibraryServer server)
        {
            _server = server;
            OnDisconnected = _server.OnDisconnected;
        }

        public async Task Disconnect()
        {
            var t = OnDisconnected.Wait(Multiverse.Timeout);
            _server.Disconnect();
            await t;
        }
    }
}