using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reaction;
using UnityEngine;

namespace Multiverse
{
    public class MvMatchmaker : IMvLibraryMatchmaker
    {
        private readonly IMvLibraryMatchmaker _matchmaker;

        private Task _lookForMatchesTask;
        private bool _lookingForMatches;

        public RxnSet<IMvMatch> Matches { get; }

        public MvMatchmaker(IMvLibraryMatchmaker matchmaker)
        {
            _matchmaker = matchmaker;
            Matches = new RxnSet<IMvMatch>();
        }

        #region IMvLibraryMatchmaker

        public bool Connected => _matchmaker.Connected;

        public async Task Connect()
        {
            await _matchmaker.Connect();
            StartLookingForMatches();
        }

        public async Task Disconnect()
        {
            StopLookingForMatches();
            await _matchmaker.Disconnect();
        }

        public async Task CreateMatch(string matchName = null, int maxPlayers = int.MaxValue)
        {
            StopLookingForMatches();
            await _matchmaker.CreateMatch(matchName, maxPlayers);
        }

        public async Task JoinMatch(IMvMatch match)
        {
            StopLookingForMatches();
            await _matchmaker.JoinMatch(match);
        }

        public Task<IEnumerable<IMvMatch>> GetMatchList() => _matchmaker.GetMatchList();

        #endregion

        private void StartLookingForMatches()
        {
            _lookingForMatches = true;
            _lookForMatchesTask ??= LookForMatches();
        }

        private void StopLookingForMatches()
        {
            _lookingForMatches = false;
            _lookForMatchesTask = null;
            Matches.AsOwner.Clear();
        }

        private async Task LookForMatches()
        {
            while (_lookingForMatches && Connected)
            {
                Debug.Log("Looking for matches...");
                var newMatches = (await GetMatchList()).ToArray();
                Matches.AsOwner.AddRange(newMatches.Except(Matches));
                Matches.AsOwner.RemoveRange(Matches.Except(newMatches));
                await Task.Delay(3000);
            }
        }
    }
}