using System.Threading.Tasks;
using Reaction;

namespace Multiverse
{
    public interface IMvLibraryClient
    {
       MvConnection LocalConnection { get; }
        RxnSet<MvConnection> Connections { get; }

        RxnEvent OnDisconnected { get; }

        Task Disconnect();
    }
}