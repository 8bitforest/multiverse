using System.Threading.Tasks;
using Reaction;

namespace Multiverse
{
    public interface IMvLibraryServer
    {
        RxnEvent OnDisconnected { get; }
        
        Task Disconnect();
    }
}