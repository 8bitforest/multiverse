using System.Threading.Tasks;
using Reaction;

namespace Multiverse
{
    public interface IMvLibraryClient
    {
        IMvConnection LocalConnection { get; }
        RxnSet<IMvConnection> Connections { get; }

        RxnEvent OnDisconnected { get; }

        Task Disconnect();
    }
}