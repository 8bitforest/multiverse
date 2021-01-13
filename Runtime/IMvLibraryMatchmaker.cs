using System.Collections.Generic;
using System.Threading.Tasks;

namespace Multiverse
{
    // TODO: Rename matches to rooms
    public interface IMvLibraryMatchmaker
    {
        bool Connected { get; }
        Task Connect();
        Task Disconnect();
        Task CreateMatch(string matchName = null, int maxPlayers = int.MaxValue);
        Task JoinMatch(IMvMatch match);
        Task<IEnumerable<IMvMatch>> GetMatchList();
    }
}