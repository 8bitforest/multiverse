using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Multiverse.Common
{
    public class MvMatchmaker : IMvLibraryMatchmaker
    {
        private readonly IMvLibraryMatchmaker _matchmaker;

        private Task _lookForMatchesTask;
        private bool _lookingForMatches = false;

        public IEnumerable<IMvMatch> Matches { get; private set; }

        public MvMatchmaker(IMvLibraryMatchmaker matchmaker)
        {
            _matchmaker = matchmaker;
            Matches = new List<IMvMatch>();
            
            // TODO: Use events to listen for connect/disconnect/match join/match leave
        }

        public bool Connected => _matchmaker.Connected;
        public Task Connect() => _matchmaker.Connect();
        public Task Disconnect() => _matchmaker.Disconnect();
        public Task CreateMatch(string matchName = null, int maxPlayers = int.MaxValue) =>
            _matchmaker.CreateMatch(matchName, maxPlayers);
        public Task JoinMatch(IMvMatch match) => _matchmaker.JoinMatch(match);
        public Task<IEnumerable<IMvMatch>> GetMatchList() => _matchmaker.GetMatchList();

        private async Task LookForMatches()
        {
            while (_lookingForMatches)
            {
                Matches = await GetMatchList();
                Thread.Sleep(3000);
            }
        }
    }
}