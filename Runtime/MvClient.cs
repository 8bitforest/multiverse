using System.Threading.Tasks;
using Reaction;

namespace Multiverse
{
    public class MvClient
    {
        public RxnEvent OnDisconnected { get; }

        private readonly IMvLibraryClient _client;

        public MvClient(IMvLibraryClient client)
        {
            _client = client;
            OnDisconnected = _client.OnDisconnected;
        }

        public async Task Disconnect()
        {
            var t = OnDisconnected.Wait(Multiverse.Timeout);
            _client.Disconnect();
            await t;
        }
    }
}