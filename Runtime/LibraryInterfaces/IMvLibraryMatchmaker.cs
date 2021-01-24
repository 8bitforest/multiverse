using System.Collections.Generic;
using System.Threading.Tasks;
using Reaction;

namespace Multiverse.LibraryInterfaces
{
    // TODO: Rename matches to rooms
    public interface IMvLibraryMatchmaker
    {
        bool Connected { get; }
        public RxnEvent OnDisconnected { get; }
        
        Task Connect();
        Task Disconnect();
        Task CreateMatch(string matchName = null, int maxPlayers = int.MaxValue);
        Task JoinMatch(MvMatch match);
        Task<IEnumerable<MvMatch>> GetMatchList();
    }
}