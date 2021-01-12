using System.Collections.Generic;
using System.Threading.Tasks;

namespace Multiverse.Common
{
    public interface IMvMatchmaker
    {
        bool Connected { get; }
        Task Connect();
        Task CreateMatch(int maxPlayers);
        Task JoinMatch(IMvMatch match);
        Task<IEnumerable<IMvMatch>> GetMatchList();
    }
}